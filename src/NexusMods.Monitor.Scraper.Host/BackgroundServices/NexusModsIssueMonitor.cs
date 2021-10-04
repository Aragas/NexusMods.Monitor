using BetterHostedServices;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Application.Queries.Issues;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues;
using NexusMods.Monitor.Scraper.Application.Queries.Subscriptions;

using NodaTime;

using Polly;

using RateLimiter;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Host.BackgroundServices
{
    public sealed class NexusModsIssueMonitor : CriticalBackgroundService
    {
        private readonly ILogger _logger;
        private readonly IClock _clock;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeLimiter _timeLimiter;

        public NexusModsIssueMonitor(ILogger<NexusModsIssueMonitor> logger, IClock clock, IServiceScopeFactory scopeFactory, IApplicationEnder applicationEnder) : base(applicationEnder)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _timeLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromSeconds(90));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scope = _logger.BeginScope("Service: {service}", nameof(NexusModsIssueMonitor));

            stoppingToken.Register(() => _logger.LogInformation("Issues processing is stopping"));

            var policy = Policy.Handle<Exception>(ex => ex.GetType() != typeof(TaskCanceledException))
                .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromMinutes(10),
                    (ex, time) =>
                    {
                        _logger.LogError(ex, "Exception during issues processing. Waiting {time}...", time);
                    });

            while (!stoppingToken.IsCancellationRequested)
            {
                await policy.ExecuteAsync(async token =>
                {
                    await _timeLimiter.Enqueue(async () => await ProcessIssues(token), token);
                }, stoppingToken);
            }

            scope.Dispose();
        }

        private async Task ProcessIssues(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var subscriptionQueries = scope.ServiceProvider.GetRequiredService<ISubscriptionQueries>();
            var issueQueries = scope.ServiceProvider.GetRequiredService<IIssueQueries>();
            var nexusModsIssueQueries = scope.ServiceProvider.GetRequiredService<INexusModsIssueQueries>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await foreach (var (nexusModsGameId, nexusModsModId) in subscriptionQueries.GetAllAsync(ct).Distinct(new SubscriptionViewModelComparer()).WithCancellation(ct))
            {
                var nexusModsIssues = await nexusModsIssueQueries.GetAllAsync(nexusModsGameId, nexusModsModId, ct).ToDictionaryAsync(x => x.NexusModsIssue.Id, x => x, ct);
                if (nexusModsIssues.Count == 0)
                    continue;

                var databaseIssues = await issueQueries.GetAllAsync(nexusModsGameId, nexusModsModId, ct).ToDictionaryAsync(x => x.Id, x => x, ct);

                var nexusModsIssuesKeys = nexusModsIssues.Keys.ToImmutableHashSet();
                var databaseIssuesKeys = databaseIssues.Keys.ToImmutableHashSet();

                var newIssues = nexusModsIssuesKeys.Except(databaseIssuesKeys).Select(key => nexusModsIssues[key]);
                var deletedIssues = databaseIssuesKeys.Except(nexusModsIssuesKeys).Select(key => databaseIssues[key]);
                var existingIssues = nexusModsIssuesKeys.Intersect(databaseIssuesKeys).Select(key => (databaseIssues[key], nexusModsIssues[key]));

                var now = _clock.GetCurrentInstant();

                foreach (var issueRoot in newIssues)
                {
                    issueRoot.SetContent(await nexusModsIssueQueries.GetContentAsync(issueRoot.NexusModsIssue.Id, ct));
                    await issueRoot.SetRepliesAsync(nexusModsIssueQueries.GetRepliesAsync(issueRoot.NexusModsIssue.Id, ct));

                    var issueStatus = await issueQueries.GetStatusAsync(issueRoot.NexusModsIssue.Status.Id, ct);
                    var issuePriority = await issueQueries.GetPriorityAsync(issueRoot.NexusModsIssue.Priority.Id, ct);

                    if (now - issueRoot.NexusModsIssue.LastPost < Duration.FromDays(1))
                        await mediator.Send(IssueAddNewCommand.FromViewModel(issueRoot, issueStatus, issuePriority), ct);
                    else
                        await mediator.Send(IssueAddCommand.FromViewModel(issueRoot, issueStatus, issuePriority), ct);
                }

                foreach (var (databaseIssue, nexusModsIssueRoot) in existingIssues)
                {
                    if (databaseIssue.Status != nexusModsIssueRoot.NexusModsIssue.Status.Id)
                    {
                        await mediator.Send(new IssueChangeStatusCommand(databaseIssue.Id, nexusModsIssueRoot.NexusModsIssue.Status.Id), ct);
                    }

                    if (databaseIssue.Priority != nexusModsIssueRoot.NexusModsIssue.Priority.Id)
                    {
                        await mediator.Send(new IssueChangePriorityCommand(databaseIssue.Id, nexusModsIssueRoot.NexusModsIssue.Priority.Id), ct);
                    }

                    if (databaseIssue.IsClosed != nexusModsIssueRoot.NexusModsIssue.IsClosed)
                    {
                        await mediator.Send(new IssueChangeIsClosedCommand(databaseIssue.Id, nexusModsIssueRoot.NexusModsIssue.IsClosed), ct);
                    }

                    if (databaseIssue.IsPrivate != nexusModsIssueRoot.NexusModsIssue.IsPrivate)
                    {
                        await mediator.Send(new IssueChangeIsPrivateCommand(databaseIssue.Id, nexusModsIssueRoot.NexusModsIssue.IsPrivate), ct);
                    }

                    if (databaseIssue.TimeOfLastPost < nexusModsIssueRoot.NexusModsIssue.LastPost)
                    {
                        var newReplies = nexusModsIssueRoot.NexusModsIssueReplies.Where(x => databaseIssue.Replies.All(y => y.Id != x.Id));
                        var deletedReplies = databaseIssue.Replies.Where(x => nexusModsIssueRoot.NexusModsIssueReplies.All(y => y.Id != x.Id)).ToImmutableArray();

                        foreach (var issueReply in newReplies)
                        {
                            if (now - issueReply.Time < Duration.FromMinutes(2))
                                await mediator.Send(IssueAddNewReplyCommand.FromViewModel(nexusModsIssueRoot, issueReply), ct);
                            else
                                await mediator.Send(IssueAddReplyCommand.FromViewModel(nexusModsIssueRoot, issueReply), ct);
                        }

                        foreach (var (id, ownerId) in deletedReplies)
                        {
                            await mediator.Send(new IssueRemoveReplyCommand(ownerId, id), ct);
                        }
                    }
                }

                foreach (var issue in deletedIssues)
                {
                    await mediator.Send(new IssueRemoveCommand(issue.Id), ct);
                }
            }
        }
    }
}