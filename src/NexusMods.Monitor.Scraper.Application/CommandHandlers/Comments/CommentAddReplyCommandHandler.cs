using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Scraper.Domain.Events.Comments;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments
{
    public class CommentAddReplyCommandHandler : IRequestHandler<CommentAddReplyCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;

        public CommentAddReplyCommandHandler(ILogger<CommentAddNewCommandHandler> logger, ICommentRepository commentRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        }

        public async Task<bool> Handle(CommentAddReplyCommand message, CancellationToken cancellationToken)
        {
            var commentEntity = await _commentRepository.GetAsync(message.Id);
            if (commentEntity is null) return false;

            commentEntity.AddReplyEntity(
                message.ReplyId,
                message.Url,
                message.Author,
                message.AuthorUrl,
                message.AvatarUrl,
                message.Content,
                message.IsDeleted,
                message.TimeOfPost);

            foreach (var @event in commentEntity.DomainEvents.OfType<CommentAddedReplyEvent>().ToList())
                commentEntity.RemoveDomainEvent(@event);

            _commentRepository.Update(commentEntity);

            return await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}