using Enbiso.NLib.EventBus;

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
    public sealed class CommentChangeIsLockedCommandHandler : IRequestHandler<CommentChangeIsLockedCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentIntegrationEventPublisher _eventPublisher;

        public CommentChangeIsLockedCommandHandler(ILogger<CommentChangeIsLockedCommandHandler> logger, ICommentRepository commentRepository, ICommentIntegrationEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(CommentChangeIsLockedCommand message, CancellationToken ct)
        {
            var commentEntity = await _commentRepository.GetAsync(message.Id);
            if (commentEntity is null)
            {
                _logger.LogError("Comment with Id {Id} was not found.", message.Id);
                return false;
            }

            if (commentEntity.IsLocked != message.IsLocked)
            {
                _logger.LogError("Comment with Id {Id} has already the correct IsLocked value.", message.Id);
                return false;
            }

            var oldIsLocked = commentEntity.IsLocked;
            commentEntity.SetIsLocked(message.IsLocked);
            _commentRepository.Update(commentEntity);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                var commentDTO = Mapper.Map(commentEntity);
                await _eventPublisher.Publish(new CommentChangedIsLockedIntegrationEvent(commentDTO, oldIsLocked), ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}