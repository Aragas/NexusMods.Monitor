using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments
{
    public sealed class CommentAddNewCommandHandler : IRequestHandler<CommentAddNewCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentIntegrationEventPublisher _eventPublisher;

        public CommentAddNewCommandHandler(ILogger<CommentAddNewCommandHandler> logger, ICommentRepository commentRepository, ICommentIntegrationEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(CommentAddNewCommand message, CancellationToken ct)
        {
            if (await _commentRepository.GetAsync(message.Id) is { } existingCommentEntity)
            {
                if (existingCommentEntity.IsDeleted)
                {
                    existingCommentEntity.Return();
                    return await _commentRepository.UnitOfWork.SaveEntitiesAsync(ct);
                }

                _logger.LogError("Comment with Id {Id} already exist, is not deleted.", message.Id);
                return false;
            }

            var commentEntity = Mapper.Map(message);
            _commentRepository.Add(commentEntity);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                var commentDTO = Mapper.Map(commentEntity);
                await _eventPublisher.Publish(new CommentAddedIntegrationEvent(commentDTO), ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}