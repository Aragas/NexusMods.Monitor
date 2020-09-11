using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Scraper.Domain.Comparators;
using NexusMods.Monitor.Scraper.Infrastructure.ComposableAsync;
using NexusMods.Monitor.Scraper.Infrastructure.Models.Issues;
using NexusMods.Monitor.Scraper.Infrastructure.RateLimiter;

using NodaTime;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.BackgroundServices
{
    public class NexusModsIssueMonitor : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IClock _clock;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeLimiter _timeLimiter;

        public NexusModsIssueMonitor(ILogger<NexusModsIssueMonitor> logger, IClock clock, IServiceScopeFactory scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _timeLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromMinutes(1));
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await _timeLimiter;


                    using var scope = _scopeFactory.CreateScope();
                    var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
                    var issueRepository = scope.ServiceProvider.GetRequiredService<IIssueRepository>();
                    var nexusModsIssuesRepository = scope.ServiceProvider.GetRequiredService<INexusModsIssuesRepository>();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    await foreach (var subscriptionEntity in subscriptionRepository.GetAllAsync().Distinct(new SubscriptionEntityComparer()).WithCancellation(ct))
                    {
                        var nexusModsIssues = await nexusModsIssuesRepository.GetIssuesAsync(subscriptionEntity.NexusModsGameId, subscriptionEntity.NexusModsModId).ToListAsync(ct);
                        var databaseIssues = await issueRepository.GetAll().Where(x => x.NexusModsGameId == subscriptionEntity.NexusModsGameId && x.NexusModsModId == subscriptionEntity.NexusModsModId).ToListAsync(ct);

                        var newIssues = nexusModsIssues.Where(x => databaseIssues.All(y => y.Id != x.NexusModsIssue.Id));
                        var deletedIssues = databaseIssues.Where(x => nexusModsIssues.All(y => y.NexusModsIssue.Id != x.Id)).ToList();
                        var existingIssues = nexusModsIssues.Select(nmie => databaseIssues.Find(y => y?.Id == nmie.NexusModsIssue.Id) is { } die
                             ? (DatabaseIssueEntity: die, NexusModsIssueEntity: nmie)
                             : default).Where(tuple => tuple != default).ToList();

                        var now = _clock.GetCurrentInstant();

                        foreach (var nexusModsIssueRoot in newIssues)
                        {
                            nexusModsIssueRoot.SetContent(await nexusModsIssuesRepository.GetIssueContentAsync(nexusModsIssueRoot.NexusModsIssue.Id));
                            await nexusModsIssueRoot.SetReplies(nexusModsIssuesRepository.GetIssueRepliesAsync(nexusModsIssueRoot.NexusModsIssue.Id));
                            if (now - nexusModsIssueRoot.NexusModsIssue.LastPost < Duration.FromDays(1))
                                await mediator.Send(new IssueAddNewCommand(nexusModsIssueRoot), ct);
                            else
                                await mediator.Send(new IssueAddCommand(nexusModsIssueRoot), ct);
                        }

                        foreach (var (databaseIssueEntity, nexusModsIssueRoot) in existingIssues)
                        {
                            if (databaseIssueEntity.Status.Id != nexusModsIssueRoot.NexusModsIssue.Status.Id)
                            {
                                await mediator.Send(new IssueChangeStatusCommand(databaseIssueEntity.Id, nexusModsIssueRoot.NexusModsIssue.Status.Id), ct);
                            }

                            if (databaseIssueEntity.Priority.Id != nexusModsIssueRoot.NexusModsIssue.Priority.Id)
                            {
                                await mediator.Send(new IssueChangePriorityCommand(databaseIssueEntity.Id, nexusModsIssueRoot.NexusModsIssue.Priority.Id), ct);
                            }

                            if (databaseIssueEntity.IsClosed != nexusModsIssueRoot.NexusModsIssue.IsClosed)
                            {
                                await mediator.Send(new IssueChangeIsClosedCommand(databaseIssueEntity.Id, nexusModsIssueRoot.NexusModsIssue.IsClosed), ct);
                            }

                            if (databaseIssueEntity.IsPrivate != nexusModsIssueRoot.NexusModsIssue.IsPrivate)
                            {
                                await mediator.Send(new IssueChangeIsPrivateCommand(databaseIssueEntity.Id, nexusModsIssueRoot.NexusModsIssue.IsPrivate), ct);
                            }

                            if (/*databaseIssueEntity.Replies.Count() < nexusModsIssueRoot.NexusModsIssue.ReplyCount ||*/ databaseIssueEntity.TimeOfLastPost < nexusModsIssueRoot.NexusModsIssue.LastPost)
                            {
                                if (nexusModsIssueRoot.NexusModsIssueReplies is null)
                                    await nexusModsIssueRoot.SetReplies(nexusModsIssuesRepository.GetIssueRepliesAsync(nexusModsIssueRoot.NexusModsIssue.Id));

                                var newReplies = nexusModsIssueRoot.NexusModsIssueReplies!.Where(x => databaseIssueEntity.Replies.All(y => y.Id != x.Id));
                                var deletedReplies = databaseIssueEntity.Replies.Where(x => nexusModsIssueRoot.NexusModsIssueReplies!.All(y => y.Id != x.Id)).ToList();

                                foreach (var nexusModsIssueReply in newReplies)
                                {
                                    if (now - nexusModsIssueReply.Time < Duration.FromMinutes(2))
                                        await mediator.Send(new IssueAddNewReplyCommand(nexusModsIssueRoot, nexusModsIssueReply), ct);
                                    else
                                        await mediator.Send(new IssueAddReplyCommand(nexusModsIssueRoot, nexusModsIssueReply), ct);
                                }

                                foreach (var issueReplyEntity in deletedReplies)
                                {
                                    await mediator.Send(new IssueRemoveReplyCommand(issueReplyEntity.OwnerId, issueReplyEntity.Id), ct);
                                }
                            }

                            if (nexusModsIssueRoot.NexusModsIssueReplies is { })
                            {

                            }
                        }

                        foreach (var issueEntity in deletedIssues)
                        {
                            await mediator.Send(new IssueRemoveCommand(issueEntity.Id), ct);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception in execution flow!");
            }
        }
    }
}