using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments
{
    public sealed class CommentAddCommandHandler : IRequestHandler<CommentAddCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ICommentRepository _commentRepository;

        public CommentAddCommandHandler(ILogger<CommentAddCommandHandler> logger, ICommentRepository commentRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentRepository = commentRepository ?? throw new ArgumentNullException(nameof(commentRepository));
        }

        public async Task<bool> Handle(CommentAddCommand message, CancellationToken ct)
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

            return await _commentRepository.UnitOfWork.SaveEntitiesAsync(ct);
        }
    }
}