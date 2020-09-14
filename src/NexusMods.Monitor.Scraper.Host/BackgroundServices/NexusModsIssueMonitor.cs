using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Application.Queries.Issues;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues;
using NexusMods.Monitor.Scraper.Application.Queries.Subscriptions;

using NodaTime;

using Polly;

using RateLimiter;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Host.BackgroundServices
{
    public sealed class NexusModsIssueMonitor : BackgroundService
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
            _timeLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromSeconds(90));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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
        }

        private async Task ProcessIssues(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var subscriptionQueries = scope.ServiceProvider.GetRequiredService<ISubscriptionQueries>();
            var issueQueries = scope.ServiceProvider.GetRequiredService<IIssueQueries>();
            var nexusModsIssueQueries = scope.ServiceProvider.GetRequiredService<INexusModsIssueQueries>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await foreach (var subscription in subscriptionQueries.GetAllAsync().Distinct(new SubscriptionViewModelComparer()).WithCancellation(ct))
            {
                var nexusModsIssues = await nexusModsIssueQueries.GetAllAsync(subscription.NexusModsGameId, subscription.NexusModsModId).ToListAsync(ct);
                var databaseIssues = await issueQueries.GetAllAsync().Where(x =>
                    x.NexusModsGameId == subscription.NexusModsGameId &&
                    x.NexusModsModId == subscription.NexusModsModId).ToListAsync(ct);

                var newIssues = nexusModsIssues.Where(x => databaseIssues.All(y => y.Id != x.NexusModsIssue.Id));
                var deletedIssues = databaseIssues.Where(x => nexusModsIssues.All(y => y.NexusModsIssue.Id != x.Id)).ToList();
                var existingIssues = nexusModsIssues.Select(nmie => databaseIssues.Find(y => y.Id == nmie.NexusModsIssue.Id) is { } die
                    ? (DatabaseIssueEntity: die, NexusModsIssueEntity: nmie)
                    : default).Where(tuple => tuple != default).ToList();

                var now = _clock.GetCurrentInstant();

                foreach (var issueRoot in newIssues)
                {
                    issueRoot.SetContent(await nexusModsIssueQueries.GetContentAsync(issueRoot.NexusModsIssue.Id));
                    await issueRoot.SetReplies(nexusModsIssueQueries.GetRepliesAsync(issueRoot.NexusModsIssue.Id));

                    if (now - issueRoot.NexusModsIssue.LastPost < Duration.FromDays(1))
                        await mediator.Send(new IssueAddNewCommand(issueRoot), ct);
                    else
                        await mediator.Send(new IssueAddCommand(issueRoot), ct);
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

                    if (/*databaseIssueEntity.Replies.Count() < nexusModsIssueRoot.NexusModsIssue.ReplyCount ||*/databaseIssue.TimeOfLastPost < nexusModsIssueRoot.NexusModsIssue.LastPost)
                    {
                        if (nexusModsIssueRoot.NexusModsIssueReplies is null)
                            await nexusModsIssueRoot.SetReplies(nexusModsIssueQueries.GetRepliesAsync(nexusModsIssueRoot.NexusModsIssue.Id));

                        var newReplies = nexusModsIssueRoot.NexusModsIssueReplies!.Where(x => databaseIssue.Replies.All(y => y.Id != x.Id));
                        var deletedReplies = databaseIssue.Replies.Where(x => nexusModsIssueRoot.NexusModsIssueReplies!.All(y => y.Id != x.Id)).ToList();

                        foreach (var issueReply in newReplies)
                        {
                            if (now - issueReply.Time < Duration.FromMinutes(2))
                                await mediator.Send(new IssueAddNewReplyCommand(nexusModsIssueRoot, issueReply), ct);
                            else
                                await mediator.Send(new IssueAddReplyCommand(nexusModsIssueRoot, issueReply), ct);
                        }

                        foreach (var issueReply in deletedReplies)
                        {
                            await mediator.Send(new IssueRemoveReplyCommand(issueReply.OwnerId, issueReply.Id), ct);
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