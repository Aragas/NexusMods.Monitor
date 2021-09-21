using Enbiso.NLib.EventBus;

using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;
using NexusMods.Monitor.Shared.Application.Models;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments
{
    public sealed class CommentRemoveReplyCommandHandler : IRequestHandler<CommentRemoveReplyCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly IEventPublisher _eventPublisher;

        public CommentRemoveReplyCommandHandler(ILogger<CommentRemoveReplyCommandHandler> logger, ICommentRepository commentRepository, IEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(CommentRemoveReplyCommand message, CancellationToken ct)
        {
            var commentEntity = await _commentRepository.GetAsync(message.Id);
            if (commentEntity is null)
            {
                _logger.LogError("Comment with Id {Id} was not found.", message.Id);
                return false;
            }

            if (commentEntity.Replies.All(r => r.Id != message.Id))
            {
                _logger.LogError("Comment with Id {Id} doesn't have the reply! CommentReply Id {ReplyId}", message.Id, message.ReplyId);
                return false;
            }

            var commentReplyDTO = Mapper.Map(commentEntity.Replies.First(x => x.Id == message.ReplyId));

            commentEntity.RemoveReply(message.ReplyId);
            _commentRepository.Update(commentEntity);

            var commentDTO = Mapper.Map(commentEntity);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                await _eventPublisher.Publish(new CommentRemovedReplyIntegrationEvent(commentDTO, commentReplyDTO), "comment_events", ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}