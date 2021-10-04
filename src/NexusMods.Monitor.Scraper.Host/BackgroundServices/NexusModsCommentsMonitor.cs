using BetterHostedServices;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Application.Queries.Comments;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments;
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
    public sealed class NexusModsCommentsMonitor : CriticalBackgroundService
    {
        private readonly ILogger _logger;
        private readonly IClock _clock;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeLimiter _timeLimiter;

        public NexusModsCommentsMonitor(ILogger<NexusModsCommentsMonitor> logger, IClock clock, IServiceScopeFactory scopeFactory, IApplicationEnder applicationEnder) : base(applicationEnder)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _timeLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromSeconds(90));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scope = _logger.BeginScope("Service: {service}", nameof(NexusModsCommentsMonitor));

            stoppingToken.Register(() => _logger.LogInformation("Comments processing is stopping"));

            var policy = Policy.Handle<Exception>(ex => ex.GetType() != typeof(TaskCanceledException))
                .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromMinutes(10),
                    (ex, time) =>
                    {
                        _logger.LogError(ex, "Exception during comments processing. Waiting {time}...", time);
                    });

            while (!stoppingToken.IsCancellationRequested)
            {
                await policy.ExecuteAsync(async token =>
                {
                    await _timeLimiter.Enqueue(async () => await ProcessComments(token), token);
                }, stoppingToken);
            }

            scope.Dispose();
        }

        private async Task ProcessComments(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var subscriptionQueries = scope.ServiceProvider.GetRequiredService<ISubscriptionQueries>();
            var commentQueries = scope.ServiceProvider.GetRequiredService<ICommentQueries>();
            var nexusModsCommentQueries = scope.ServiceProvider.GetRequiredService<INexusModsCommentQueries>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await foreach (var (nexusModsGameId, nexusModsModId) in subscriptionQueries.GetAllAsync(ct).Distinct(new SubscriptionViewModelComparer()).WithCancellation(ct))
            {
                var nexusModsComments = await nexusModsCommentQueries.GetAllAsync(nexusModsGameId, nexusModsModId, ct).ToDictionaryAsync(x => x.NexusModsComment.Id, x => x, ct);
                if (nexusModsComments.Count == 0)
                    continue;

                var databaseComments = await commentQueries.GetAllAsync(nexusModsGameId, nexusModsModId, ct).ToDictionaryAsync(x => x.Id, x => x, ct);

                var nexusModsCommentsKeys = nexusModsComments.Keys.ToImmutableHashSet();
                var databaseCommentsKeys = databaseComments.Keys.ToImmutableHashSet();

                var newComments = nexusModsCommentsKeys.Except(databaseCommentsKeys).Select(key => nexusModsComments[key]);
                var deletedComments = databaseCommentsKeys.Except(nexusModsCommentsKeys).Select(key => databaseComments[key]);
                var existingComments = nexusModsCommentsKeys.Intersect(databaseCommentsKeys).Select(key => (databaseComments[key], nexusModsComments[key]));

                var now = _clock.GetCurrentInstant();

                foreach (var commentRoot in newComments)
                {
                    if (now - commentRoot.NexusModsComment.Post < Duration.FromDays(1))
                        await mediator.Send(CommentAddNewCommand.FromViewModel(commentRoot), ct);
                    else
                        await mediator.Send(CommentAddCommand.FromViewModel(commentRoot), ct);
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


                    var newReplies = nexusModsCommentRoot.NexusModsComment.Replies.Where(x => databaseComment.Replies.All(y => y.Id != x.Id));
                    var deletedReplies = databaseComment.Replies.Where(x => nexusModsCommentRoot.NexusModsComment.Replies.All(y => y.Id != x.Id)).ToImmutableArray();

                    foreach (var commentReply in newReplies)
                    {
                        if (now - commentReply.Post < Duration.FromMinutes(2))
                            await mediator.Send(CommentAddNewReplyCommand.FromViewModel(nexusModsCommentRoot, commentReply), ct);
                        else
                            await mediator.Send(CommentAddReplyCommand.FromViewModel(nexusModsCommentRoot, commentReply), ct);
                    }

                    foreach (var (id, ownerId) in deletedReplies)
                    {
                        await mediator.Send(new CommentRemoveReplyCommand(ownerId, id), ct);
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