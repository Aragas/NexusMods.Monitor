using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments;
using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public sealed class IssueAddReplyCommandHandler : IRequestHandler<IssueAddReplyCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;

        public IssueAddReplyCommandHandler(ILogger<CommentAddNewCommandHandler> logger, IIssueRepository issueRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
        }

        public async Task<bool> Handle(IssueAddReplyCommand message, CancellationToken cancellationToken)
        {
            var issueEntity = await _issueRepository.GetAsync(message.OwnerId);
            if (issueEntity is null)
            {
                _logger.LogError("Issue with Id {Id} was not found! IssueReply Id {ReplyId}", message.OwnerId, message.Id);
                return false;
            }

            issueEntity.AddReplyEntity(
                message.Id,
                message.Author,
                message.AuthorUrl,
                message.AvatarUrl,
                message.Content,
                message.IsDeleted,
                message.TimeOfPost);

            _issueRepository.Update(issueEntity);

            return await _issueRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}