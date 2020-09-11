using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Scraper.Domain.Comparators;
using NexusMods.Monitor.Scraper.Infrastructure.ComposableAsync;
using NexusMods.Monitor.Scraper.Infrastructure.Models.Comments;
using NexusMods.Monitor.Scraper.Infrastructure.RateLimiter;

using NodaTime;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.BackgroundServices
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

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await _timeLimiter;


                    using var scope = _scopeFactory.CreateScope();
                    var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
                    var commentRepository = scope.ServiceProvider.GetRequiredService<ICommentRepository>();
                    var nexusModsCommentsRepository = scope.ServiceProvider.GetRequiredService<INexusModsCommentsRepository>();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    await foreach (var subscriptionEntity in subscriptionRepository.GetAllAsync().Distinct(new SubscriptionEntityComparer()).WithCancellation(ct))
                    {
                        var nexusModsComments = await nexusModsCommentsRepository.GetCommentsAsync(subscriptionEntity.NexusModsGameId, subscriptionEntity.NexusModsModId).ToListAsync(ct);
                        var databaseComments = await commentRepository.GetAll().Where(x => x.NexusModsGameId == subscriptionEntity.NexusModsGameId && x.NexusModsModId == subscriptionEntity.NexusModsModId).ToListAsync(ct);

                        var newComments = nexusModsComments.Where(x => databaseComments.All(y => y.Id != x.NexusModsComment.Id));
                        var deletedComments = databaseComments.Where(x => nexusModsComments.All(y => y.NexusModsComment.Id != x.Id)).ToList();
                        var existingComments = nexusModsComments.Select(nmce => databaseComments.Find(y => y?.Id == nmce.NexusModsComment.Id) is { } dce
                             ? (DatabaseCommentEntity: dce, NexusModsCommentEntity: nmce)
                             : default).Where(tuple => tuple != default).ToList();

                        var now = _clock.GetCurrentInstant();

                        foreach (var nexusModsCommentRoot in newComments)
                        {
                            if (now - nexusModsCommentRoot.NexusModsComment.Post < Duration.FromDays(1))
                                await mediator.Send(new CommentAddNewCommand(nexusModsCommentRoot), ct);
                            else
                                await mediator.Send(new CommentAddCommand(nexusModsCommentRoot), ct);
                        }

                        foreach (var (databaseCommentEntity, nexusModsCommentRoot) in existingComments)
                        {
                            if (databaseCommentEntity.IsLocked != nexusModsCommentRoot.NexusModsComment.IsLocked)
                            {
                                await mediator.Send(new CommentChangeIsLockedCommand(databaseCommentEntity.Id, nexusModsCommentRoot.NexusModsComment.IsLocked), ct);
                            }
                            if (databaseCommentEntity.IsSticky != nexusModsCommentRoot.NexusModsComment.IsSticky)
                            {
                                await mediator.Send(new CommentChangeIsStickyCommand(databaseCommentEntity.Id, nexusModsCommentRoot.NexusModsComment.IsSticky), ct);
                            }


                            var newReplies = nexusModsCommentRoot.NexusModsComment.Children.Where(x => databaseCommentEntity.Replies.All(y => y.Id != x.Id));
                            var deletedReplies = databaseCommentEntity.Replies.Where(x => nexusModsCommentRoot.NexusModsComment.Children.All(y => y.Id != x.Id)).ToList();

                            foreach (var nexusModsCommentReply in newReplies)
                            {
                                if (now - nexusModsCommentReply.Post < Duration.FromMinutes(2))
                                    await mediator.Send(new CommentAddNewReplyCommand(nexusModsCommentRoot, nexusModsCommentReply), ct);
                                else
                                    await mediator.Send(new CommentAddReplyCommand(nexusModsCommentRoot, nexusModsCommentReply), ct);
                            }

                            foreach (var commentReplyEntity in deletedReplies)
                            {
                                await mediator.Send(new CommentRemoveReplyCommand(commentReplyEntity.OwnerId, commentReplyEntity.Id), ct);
                            }
                        }

                        foreach (var commentEntity in deletedComments)
                        {
                            await mediator.Send(new CommentRemoveCommand(commentEntity.Id), ct);
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