using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments
{
    public sealed class CommentAddReplyCommandHandler : IRequestHandler<CommentAddReplyCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;

        public CommentAddReplyCommandHandler(ILogger<CommentAddReplyCommandHandler> logger, ICommentRepository commentRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        }

        public async Task<bool> Handle(CommentAddReplyCommand message, CancellationToken ct)
        {
            if (await _commentRepository.GetAsync(message.Id) is not { } commentEntity)
            {
                _logger.LogError("Comment with Id {Id} was not found. CommentReply Id {ReplyId}.", message.Id, message.ReplyId);
                return false;
            }

            if (commentEntity.Replies.FirstOrDefault(r => r.Id == message.ReplyId) is { } existingReplyEntity)
            {
                _logger.LogError("Comment with Id {Id} has already the reply! Existing: {@ExistingReply}, new: {@Message}", message.Id, existingReplyEntity, message);
                return false;
            }

            commentEntity.AddReplyEntity(message.ReplyId, message.Url, message.Author, message.AuthorUrl, message.AvatarUrl, message.Content, false, message.TimeOfPost);
            _commentRepository.Update(commentEntity);

            return await _commentRepository.UnitOfWork.SaveEntitiesAsync(ct);
        }
    }
}