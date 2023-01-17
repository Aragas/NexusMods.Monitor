using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Application.Queries.Issues;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues;
using NexusMods.Monitor.Scraper.Application.Queries.Subscriptions;
using NexusMods.Monitor.Shared.Common.Extensions;

using NodaTime;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application
{
    public sealed class NexusModsIssuesProcessor
    {
        private readonly ILogger _logger;
        private readonly IClock _clock;
        private readonly ISubscriptionQueries _subscriptionQueries;
        private readonly IIssueQueries _issueQueries;
        private readonly INexusModsIssueQueries _nexusModsIssueQueries;
        private readonly IMediator _mediator;

        public NexusModsIssuesProcessor(ILogger<NexusModsIssuesProcessor> logger, IClock clock, ISubscriptionQueries subscriptionQueries, IIssueQueries issueQueries, INexusModsIssueQueries nexusModsIssueQueries, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
            _issueQueries = issueQueries ?? throw new ArgumentNullException(nameof(issueQueries));
            _nexusModsIssueQueries = nexusModsIssueQueries ?? throw new ArgumentNullException(nameof(nexusModsIssueQueries));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Process(CancellationToken ct)
        {
            await foreach (var (nexusModsGameId, nexusModsModId) in _subscriptionQueries.GetAllAsync(ct).Distinct(new SubscriptionViewModelComparer()).WithCancellation(ct))
            {
                var nexusModsIssues = await _nexusModsIssueQueries.GetAllAsync(nexusModsGameId, nexusModsModId, ct).ToDictionaryAsync(x => x.Issue.Id, x => x, ct);
                var databaseIssues = await _issueQueries.GetAllAsync(nexusModsGameId, nexusModsModId, ct).ToDictionaryAsync(x => x.Id, x => x, ct);

                var nexusModsIssuesKeys = nexusModsIssues.Keys.ToHashSet();
                var databaseIssuesKeys = databaseIssues.Keys.ToHashSet();
                var now = _clock.GetCurrentInstant();

                var newIssues = nexusModsIssuesKeys.Except(databaseIssuesKeys).Select(key => nexusModsIssues[key]);
                foreach (var issueRoot in newIssues)
                {
                    // Fill issue with content
                    var filledIssue = issueRoot with
                    {
                        IssueContent = await _nexusModsIssueQueries.GetContentAsync(issueRoot.Issue.Id, ct),
                        IssueReplies = await _nexusModsIssueQueries.GetRepliesAsync(issueRoot.Issue.Id, ct).ToImmutableArrayAsync(ct),
                    };

                    var issueStatus = await _issueQueries.GetStatusAsync(filledIssue.Issue.Status.Id, ct);
                    var issuePriority = await _issueQueries.GetPriorityAsync(filledIssue.Issue.Priority.Id, ct);

                    if (now - filledIssue.Issue.LastPost < Duration.FromDays(1))
                        await _mediator.Send(IssueAddNewCommand.FromViewModel(filledIssue, issueStatus, issuePriority), ct);
                    else
                        await _mediator.Send(IssueAddCommand.FromViewModel(filledIssue, issueStatus, issuePriority), ct);
                }

                var existingIssues = nexusModsIssuesKeys.Intersect(databaseIssuesKeys).Select(key => (databaseIssues[key], nexusModsIssues[key]));
                foreach (var (databaseIssue, nexusModsIssueRoot) in existingIssues)
                {
                    if (databaseIssue.Status != nexusModsIssueRoot.Issue.Status.Id)
                    {
                        await _mediator.Send(new IssueChangeStatusCommand(databaseIssue.Id, nexusModsIssueRoot.Issue.Status.Id), ct);
                    }

                    if (databaseIssue.Priority != nexusModsIssueRoot.Issue.Priority.Id)
                    {
                        await _mediator.Send(new IssueChangePriorityCommand(databaseIssue.Id, nexusModsIssueRoot.Issue.Priority.Id), ct);
                    }

                    if (databaseIssue.IsClosed != nexusModsIssueRoot.Issue.IsClosed)
                    {
                        await _mediator.Send(new IssueChangeIsClosedCommand(databaseIssue.Id, nexusModsIssueRoot.Issue.IsClosed), ct);
                    }

                    if (databaseIssue.IsPrivate != nexusModsIssueRoot.Issue.IsPrivate)
                    {
                        await _mediator.Send(new IssueChangeIsPrivateCommand(databaseIssue.Id, nexusModsIssueRoot.Issue.IsPrivate), ct);
                    }

                    if (databaseIssue.TimeOfLastPost < nexusModsIssueRoot.Issue.LastPost)
                    {
                        var newReplies = nexusModsIssueRoot.IssueReplies.Where(x => databaseIssue.Replies.All(y => y.Id != x.Id));
                        var deletedReplies = databaseIssue.Replies.Where(x => nexusModsIssueRoot.IssueReplies.All(y => y.Id != x.Id)).ToImmutableArray();

                        foreach (var issueReply in newReplies)
                        {
                            if (now - issueReply.Time < Duration.FromMinutes(2))
                                await _mediator.Send(IssueAddNewReplyCommand.FromViewModel(nexusModsIssueRoot, issueReply), ct);
                            else
                                await _mediator.Send(IssueAddReplyCommand.FromViewModel(nexusModsIssueRoot, issueReply), ct);
                        }

                        foreach (var (id, ownerId) in deletedReplies)
                        {
                            await _mediator.Send(new IssueRemoveReplyCommand(ownerId, id), ct);
                        }
                    }
                }

                var deletedIssues = databaseIssuesKeys.Except(nexusModsIssuesKeys).Select(key => databaseIssues[key]);
                foreach (var issue in deletedIssues)
                {
                    await _mediator.Send(new IssueRemoveCommand(issue.Id), ct);
                }
            }
        }
    }
}