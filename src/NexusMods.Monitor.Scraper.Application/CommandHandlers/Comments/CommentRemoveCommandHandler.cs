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
    public sealed class CommentRemoveCommandHandler : IRequestHandler<CommentRemoveCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public CommentRemoveCommandHandler(ILogger<CommentRemoveCommandHandler> logger, ICommentRepository commentRepository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(CommentRemoveCommand message, CancellationToken cancellationToken)
        {
            var commentEntity = await _commentRepository.GetAsync(message.Id);
            if (commentEntity is null)
            {
                _logger.LogError("Comment with Id {Id} was not found.", message.Id);
                return false;
            }

            commentEntity.Remove();
            _commentRepository.Update(commentEntity);

            var commentDTO = _mapper.Map<CommentEntity, CommentDTO>(commentEntity);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await _eventPublisher.Publish(new CommentRemovedIntegrationEvent(commentDTO), "comment_events", cancellationToken);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}