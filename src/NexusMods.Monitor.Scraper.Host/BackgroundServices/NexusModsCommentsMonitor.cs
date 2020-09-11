using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Scraper.Domain.Comparators;
using NexusMods.Monitor.Scraper.Infrastructure.Models.Comments;
using NexusMods.Monitor.Scraper.Infrastructure.RateLimiter;

using NodaTime;

using Polly;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Host.BackgroundServices
{
    public class NexusModsCommentsMonitor : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IClock _clock;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeLimiter _timeLimiter;

        public NexusModsCommentsMonitor(ILogger<NexusModsCommentsMonitor> logger, IClock clock, IServiceScopeFactory scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _timeLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromSeconds(90));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => _logger.LogInformation("Comments processing is stopping"));

            var policy = Policy.Handle<Exception>(ex => ex.GetType() != typeof(TaskCanceledException))
                .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromMinutes(10),
                    (ex, time) =>
                    {
                        _logger.LogError(ex, "Exception during comments processing. Waiting {time} seconds...", time);
                    });

            while (!stoppingToken.IsCancellationRequested)
            {
                await policy.ExecuteAsync(async token =>
                {
                    await _timeLimiter.Enqueue(async () => await ProcessComments(token), token);
                }, stoppingToken);
            }
        }

        private async Task ProcessComments(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
            var commentRepository = scope.ServiceProvider.GetRequiredService<ICommentRepository>();
            var nexusModsCommentsRepository = scope.ServiceProvider.GetRequiredService<INexusModsCommentsRepository>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await foreach (var subscription in subscriptionRepository.GetAllAsync().Distinct(new SubscriptionEntityComparer()).WithCancellation(ct))
            {
                var nexusModsComments = await nexusModsCommentsRepository.GetCommentsAsync(subscription.NexusModsGameId, subscription.NexusModsModId).ToListAsync(ct);
                var databaseComments = await commentRepository.GetAll().Where(x =>
                    x.NexusModsGameId == subscription.NexusModsGameId &&
                    x.NexusModsModId == subscription.NexusModsModId).ToListAsync(ct);

                var newComments = nexusModsComments.Where(x => databaseComments.All(y => y.Id != x.NexusModsComment.Id));
                var deletedComments = databaseComments.Where(x => nexusModsComments.All(y => y.NexusModsComment.Id != x.Id)).ToList();
                var existingComments = nexusModsComments.Select(nmce => databaseComments.Find(y => y?.Id == nmce.NexusModsComment.Id) is { } dce
                    ? (DatabaseCommentEntity: dce, NexusModsCommentEntity: nmce)
                    : default).Where(tuple => tuple != default).ToList();

                var now = _clock.GetCurrentInstant();

                foreach (var commentRoot in newComments)
                {
                    if (now - commentRoot.NexusModsComment.Post < Duration.FromDays(1))
                        await mediator.Send(new CommentAddNewCommand(commentRoot), ct);
                    else
                        await mediator.Send(new CommentAddCommand(commentRoot), ct);
                }

                foreach (var (databaseComment, nexusModsCommentRoot) in existingComments)
                {
                    if (databaseComment.IsLocked != nexusModsCommentRoot.NexusModsComment.IsLocked)
                    {
                        await mediator.Send(new CommentChangeIsLockedCommand(databaseComment.Id, nexusModsCommentRoot.NexusModsComment.IsLocked), ct);
                    }

                    if (databaseComment.IsSticky != nexusModsCommentRoot.NexusModsComment.IsSticky)
                    {
                        await mediator.Send(new CommentChangeIsStickyCommand(databaseComment.Id, nexusModsCommentRoot.NexusModsComment.IsSticky), ct);
                    }


                    var newReplies = nexusModsCommentRoot.NexusModsComment.Children.Where(x => databaseComment.Replies.All(y => y.Id != x.Id));
                    var deletedReplies = databaseComment.Replies.Where(x => nexusModsCommentRoot.NexusModsComment.Children.All(y => y.Id != x.Id)).ToList();

                    foreach (var commentReply in newReplies)
                    {
                        if (now - commentReply.Post < Duration.FromMinutes(2))
                            await mediator.Send(new CommentAddNewReplyCommand(nexusModsCommentRoot, commentReply), ct);
                        else
                            await mediator.Send(new CommentAddReplyCommand(nexusModsCommentRoot, commentReply), ct);
                    }

                    foreach (var commentReply in deletedReplies)
                    {
                        await mediator.Send(new CommentRemoveReplyCommand(commentReply.OwnerId, commentReply.Id), ct);
                    }
                }

                foreach (var comment in deletedComments)
                {
                    await mediator.Send(new CommentRemoveCommand(comment.Id), ct);
                }
            }
        }
    }
}