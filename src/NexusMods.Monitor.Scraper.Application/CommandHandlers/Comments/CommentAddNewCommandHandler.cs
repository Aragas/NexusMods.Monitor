using AutoMapper;

using Enbiso.NLib.EventBus;

using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Shared.Application;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments
{
    public class CommentAddNewCommandHandler : IRequestHandler<CommentAddNewCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public CommentAddNewCommandHandler(ILogger<CommentAddNewCommandHandler> logger, ICommentRepository commentRepository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(CommentAddNewCommand message, CancellationToken cancellationToken)
        {
            var existingCommentEntity = await _commentRepository.GetAsync(message.Id);
            if (existingCommentEntity is { })
            {
                if (existingCommentEntity.IsDeleted)
                {
                    existingCommentEntity.Return();
                    return await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                }

                return false;
            }

            var commentEntity = new CommentEntity(
                message.Id,
                message.NexusModsGameId,
                message.NexusModsModId,
                message.Url,
                message.Author,
                message.AuthorUrl,
                message.AvatarUrl,
                message.Content,
                message.IsSticky,
                message.IsLocked,
                message.IsDeleted,
                message.TimeOfPost);

            foreach (var commentReply in message.CommentReplies)
            {
                commentEntity.AddReplyEntity(
                    commentReply.Id,
                    commentReply.Url,
                    commentReply.Author,
                    commentReply.AuthorUrl,
                    commentReply.AvatarUrl,
                    commentReply.Content,
                    commentReply.IsDeleted,
                    commentReply.TimeOfPost);
            }

            _commentRepository.Add(commentEntity);

            var commentDTO = _mapper.Map<CommentEntity, CommentDTO>(commentEntity);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await _eventPublisher.Publish(new CommentAddedIntegrationEvent(commentDTO), "comment_events", cancellationToken);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}