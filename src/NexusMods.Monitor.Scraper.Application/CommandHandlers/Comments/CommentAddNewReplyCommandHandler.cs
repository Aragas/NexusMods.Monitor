using AutoMapper;

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
    public sealed class CommentAddNewReplyCommandHandler : IRequestHandler<CommentAddNewReplyCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public CommentAddNewReplyCommandHandler(ILogger<CommentAddNewReplyCommandHandler> logger, ICommentRepository commentRepository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(CommentAddNewReplyCommand message, CancellationToken ct)
        {
            var commentEntity = await _commentRepository.GetAsync(message.Id);
            if (commentEntity is null)
            {
                _logger.LogError("Comment with Id {Id} was not found. CommentReply Id {ReplyId}.", message.Id, message.ReplyId);
                return false;
            }

            if (commentEntity.Replies.Any(r => r.Id == message.Id))
            {
                _logger.LogError("Comment with Id {Id} has already the reply! CommentReply Id {ReplyId}", message.Id, message.ReplyId);
                return false;
            }

            var commentReplyEntity = commentEntity.AddReplyEntity(
                message.ReplyId,
                message.Url,
                message.Author,
                message.AuthorUrl,
                message.AvatarUrl,
                message.Content,
                false,
                message.TimeOfPost);

            _commentRepository.Update(commentEntity);

            var commentDTO = _mapper.Map<CommentEntity, CommentDTO>(commentEntity);
            var commentReplyDTO = _mapper.Map<CommentReplyEntity, CommentReplyDTO>(commentReplyEntity);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                await _eventPublisher.Publish(new CommentAddedReplyIntegrationEvent(commentDTO, commentReplyDTO), "comment_events", ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}