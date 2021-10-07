using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments
{
    public sealed class CommentRemoveCommandHandler : IRequestHandler<CommentRemoveCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly INexusModsCommentQueries _nexusModsCommentQueries;
        private readonly ICommentIntegrationEventPublisher _eventPublisher;

        public CommentRemoveCommandHandler(ILogger<CommentRemoveCommandHandler> logger, ICommentRepository commentRepository, INexusModsCommentQueries nexusModsCommentQueries, ICommentIntegrationEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _nexusModsCommentQueries = nexusModsCommentQueries ?? throw new ArgumentNullException(nameof(nexusModsCommentQueries));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(CommentRemoveCommand message, CancellationToken ct)
        {
            if (await _commentRepository.GetAsync(message.Id) is not { } commentEntity)
            {
                _logger.LogError("Comment with Id {Id} was not found", message.Id);
                return false;
            }

            if (await _nexusModsCommentQueries.ExistsAsync(commentEntity.NexusModsGameId, commentEntity.NexusModsModId, commentEntity.Id, ct))
            {
                _logger.LogError("Comment with Id {Id} still exists in NexusMods!", message.Id);
                return false;
            }

            commentEntity.Remove();
            _commentRepository.Update(commentEntity);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                var commentDTO = Mapper.Map(commentEntity);
                await _eventPublisher.Publish(new CommentRemovedIntegrationEvent(commentDTO), ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}