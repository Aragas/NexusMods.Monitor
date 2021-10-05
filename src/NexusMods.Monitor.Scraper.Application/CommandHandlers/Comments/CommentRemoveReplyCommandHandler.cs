using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

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
        private readonly ICommentIntegrationEventPublisher _eventPublisher;

        public CommentRemoveReplyCommandHandler(ILogger<CommentRemoveReplyCommandHandler> logger, ICommentRepository commentRepository, ICommentIntegrationEventPublisher eventPublisher)
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

            if (commentEntity.Replies.All(r => r.Id != message.ReplyId))
            {
                _logger.LogError("Comment with Id {Id} doesn't have the reply! CommentReply Id {ReplyId}", message.Id, message.ReplyId);
                return false;
            }

            var commentReplyEntity = commentEntity.RemoveReplyEntity(message.ReplyId)!;

            _commentRepository.Update(commentEntity);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                var commentDTO = Mapper.Map(commentEntity);
                var commentReplyDTO = Mapper.Map(commentReplyEntity);
                await _eventPublisher.Publish(new CommentRemovedReplyIntegrationEvent(commentDTO, commentReplyDTO), ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}