using AutoMapper;

using Enbiso.NLib.EventBus;

using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;
using NexusMods.Monitor.Shared.Application.Models;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments
{
    public sealed class CommentChangeIsStickyCommandHandler : IRequestHandler<CommentChangeIsStickyCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public CommentChangeIsStickyCommandHandler(ILogger<CommentChangeIsStickyCommandHandler> logger, ICommentRepository commentRepository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(CommentChangeIsStickyCommand message, CancellationToken ct)
        {
            var commentEntity = await _commentRepository.GetAsync(message.Id);
            if (commentEntity is null)
            {
                _logger.LogError("Comment with Id {Id} was not found.", message.Id);
                return false;
            }

            if (commentEntity.IsSticky != message.IsSticky)
            {
                _logger.LogError("Comment with Id {Id} has already the correct IsSticky value.", message.Id);
                return false;
            }

            var oldIsSticky = commentEntity.IsSticky;
            commentEntity.SetIsSticky(message.IsSticky);
            _commentRepository.Update(commentEntity);

            var commentDTO = _mapper.Map<CommentEntity, CommentDTO>(commentEntity);

            if (await _commentRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                await _eventPublisher.Publish(new CommentChangedIsStickyIntegrationEvent(commentDTO, oldIsSticky), "comment_events", ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}