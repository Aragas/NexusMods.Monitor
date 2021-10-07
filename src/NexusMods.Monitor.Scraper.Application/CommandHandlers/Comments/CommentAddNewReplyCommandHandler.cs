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
    public sealed class CommentAddNewReplyCommandHandler : IRequestHandler<CommentAddNewReplyCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentIntegrationEventPublisher _eventPublisher;

        public CommentAddNewReplyCommandHandler(ILogger<CommentAddNewReplyCommandHandler> logger, ICommentRepository commentRepository, ICommentIntegrationEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(CommentAddNewReplyCommand message, CancellationToken ct)
        {
            if (await _commentRepository.GetAsync(message.Id) is not { } commentEntity)
            {
                _logger.LogError("Comment with Id {Id} was not found. CommentReply Id {ReplyId}", message.Id, message.ReplyId);
                return false;
            }

            if (commentEntity.Replies.FirstOrDefault(r => r.Id == message.ReplyId) is { } existingReplyEntity)
            {
                _logger.LogError("Comment with Id {Id} has already the reply! Existing: {@ExistingReply}, new: {@Message}", message.Id, existingReplyEntity, message);
                return false;
            }

            var commentReplyEntity = commentEntity.AddReplyEntity(message.ReplyId, message.Url, message.Author, message.AuthorUrl, message.AvatarUrl, message.Content, false, message.TimeOfPost);
            _commentRepository.Update(commentEntity);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                var commentDTO = Mapper.Map(commentEntity);
                var commentReplyDTO = Mapper.Map(commentReplyEntity);
                await _eventPublisher.Publish(new CommentAddedReplyIntegrationEvent(commentDTO, commentReplyDTO), ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}