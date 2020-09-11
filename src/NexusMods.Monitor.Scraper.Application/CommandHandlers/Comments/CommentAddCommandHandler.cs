using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments
{
    public class CommentAddCommandHandler : IRequestHandler<CommentAddCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;

        public CommentAddCommandHandler(ILogger<CommentAddNewCommandHandler> logger, ICommentRepository commentRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        }

        public async Task<bool> Handle(CommentAddCommand message, CancellationToken cancellationToken)
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

            return await _commentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}