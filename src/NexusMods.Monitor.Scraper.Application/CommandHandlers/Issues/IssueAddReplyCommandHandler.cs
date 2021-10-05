using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public sealed class IssueAddReplyCommandHandler : IRequestHandler<IssueAddReplyCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;

        public IssueAddReplyCommandHandler(ILogger<IssueAddReplyCommandHandler> logger, IIssueRepository issueRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
        }

        public async Task<bool> Handle(IssueAddReplyCommand message, CancellationToken ct)
        {
            var issueEntity = await _issueRepository.GetAsync(message.Id);
            if (issueEntity is null)
            {
                _logger.LogError("Issue with Id {Id} was not found! IssueReply Id {ReplyId}", message.Id, message.ReplyId);
                return false;
            }

            if (issueEntity.Replies.FirstOrDefault(r => r.Id == message.ReplyId) is { } existingReplyEntity)
            {
                _logger.LogError("Issue with Id {Id} has already the reply! Existing: {@ExistingIssueReply}, new: {@Message}", message.Id, existingReplyEntity, message);
                return false;
            }

            issueEntity.AddReplyEntity(message.ReplyId, message.Author, message.AuthorUrl, message.AvatarUrl, message.Content, false, message.TimeOfPost);
            _issueRepository.Update(issueEntity);

            return await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct);
        }
    }
}