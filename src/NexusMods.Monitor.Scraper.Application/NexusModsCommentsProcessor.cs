using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Application.Queries.Comments;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments;
using NexusMods.Monitor.Scraper.Application.Queries.Subscriptions;

using NodaTime;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application
{
    public sealed class NexusModsCommentsProcessor
    {
        private readonly ILogger _logger;
        private readonly IClock _clock;
        private readonly ISubscriptionQueries _subscriptionQueries;
        private readonly ICommentQueries _commentQueries;
        private readonly INexusModsCommentQueries _nexusModsCommentQueries;
        private readonly IMediator _mediator;

        public NexusModsCommentsProcessor(ILogger<NexusModsCommentsProcessor> logger, IClock clock, ISubscriptionQueries subscriptionQueries, ICommentQueries commentQueries, INexusModsCommentQueries nexusModsCommentQueries, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
            _commentQueries = commentQueries ?? throw new ArgumentNullException(nameof(commentQueries));
            _nexusModsCommentQueries = nexusModsCommentQueries ?? throw new ArgumentNullException(nameof(nexusModsCommentQueries));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Process(CancellationToken ct)
        {
            await foreach (var (nexusModsGameId, nexusModsModId) in _subscriptionQueries.GetAllAsync(ct).Distinct(new SubscriptionViewModelComparer()).WithCancellation(ct))
            {
                var nexusModsComments = await _nexusModsCommentQueries.GetAllAsync(nexusModsGameId, nexusModsModId, ct).ToDictionaryAsync(x => x.Comment.Id, x => x, ct);
                var databaseComments = await _commentQueries.GetAllAsync(nexusModsGameId, nexusModsModId, ct).ToDictionaryAsync(x => x.Id, x => x, ct);

                var nexusModsCommentsKeys = nexusModsComments.Keys.ToHashSet();
                var databaseCommentsKeys = databaseComments.Keys.ToHashSet();
                var now = _clock.GetCurrentInstant();

                var newComments = nexusModsCommentsKeys.Except(databaseCommentsKeys).Select(key => nexusModsComments[key]);
                foreach (var commentRoot in newComments)
                {
                    if (now - commentRoot.Comment.Post < Duration.FromDays(1))
                        await _mediator.Send(CommentAddNewCommand.FromViewModel(commentRoot), ct);
                    else
                        await _mediator.Send(CommentAddCommand.FromViewModel(commentRoot), ct);
                }

                var existingComments = nexusModsCommentsKeys.Intersect(databaseCommentsKeys).Select(key => (databaseComments[key], nexusModsComments[key]));
                foreach (var (databaseComment, nexusModsCommentRoot) in existingComments)
                {
                    if (databaseComment.IsLocked != nexusModsCommentRoot.Comment.IsLocked)
                    {
                        await _mediator.Send(new CommentChangeIsLockedCommand(databaseComment.Id, nexusModsCommentRoot.Comment.IsLocked), ct);
                    }

                    if (databaseComment.IsSticky != nexusModsCommentRoot.Comment.IsSticky)
                    {
                        await _mediator.Send(new CommentChangeIsStickyCommand(databaseComment.Id, nexusModsCommentRoot.Comment.IsSticky), ct);
                    }


                    var newReplies = nexusModsCommentRoot.Comment.Replies.Where(x => databaseComment.Replies.All(y => y.Id != x.Id));
                    var deletedReplies = databaseComment.Replies.Where(x => nexusModsCommentRoot.Comment.Replies.All(y => y.Id != x.Id)).ToImmutableArray();

                    foreach (var commentReply in newReplies)
                    {
                        if (now - commentReply.Post < Duration.FromMinutes(2))
                            await _mediator.Send(CommentAddNewReplyCommand.FromViewModel(nexusModsCommentRoot, commentReply), ct);
                        else
                            await _mediator.Send(CommentAddReplyCommand.FromViewModel(nexusModsCommentRoot, commentReply), ct);
                    }

                    foreach (var (id, ownerId) in deletedReplies)
                    {
                        await _mediator.Send(new CommentRemoveReplyCommand(ownerId, id), ct);
                    }
                }

                var deletedComments = databaseCommentsKeys.Except(nexusModsCommentsKeys).Select(key => databaseComments[key]);
                foreach (var comment in deletedComments)
                {
                    await _mediator.Send(new CommentRemoveCommand(comment.Id), ct);
                }
            }
        }
    }
}