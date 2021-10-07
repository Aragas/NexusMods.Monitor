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
    public sealed class CommentChangeIsStickyCommandHandler : IRequestHandler<CommentChangeIsStickyCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentIntegrationEventPublisher _eventPublisher;

        public CommentChangeIsStickyCommandHandler(ILogger<CommentChangeIsStickyCommandHandler> logger, ICommentRepository commentRepository, ICommentIntegrationEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(CommentChangeIsStickyCommand message, CancellationToken ct)
        {
            if (await _commentRepository.GetAsync(message.Id) is not { } commentEntity)
            {
                _logger.LogError("Comment with Id {Id} was not found", message.Id);
                return false;
            }

            if (commentEntity.IsSticky != message.IsSticky)
            {
                _logger.LogError("Comment with Id {Id} has already the correct IsSticky value", message.Id);
                return false;
            }

            var oldIsSticky = commentEntity.IsSticky;
            commentEntity.SetIsSticky(message.IsSticky);
            _commentRepository.Update(commentEntity);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                var commentDTO = Mapper.Map(commentEntity);
                await _eventPublisher.Publish(new CommentChangedIsStickyIntegrationEvent(commentDTO, oldIsSticky), ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}