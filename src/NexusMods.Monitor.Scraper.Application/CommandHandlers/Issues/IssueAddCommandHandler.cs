using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments;
using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Scraper.Domain.Events.Issues;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public class IssueAddCommandHandler : IRequestHandler<IssueAddCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;

        public IssueAddCommandHandler(ILogger<CommentAddNewCommandHandler> logger, IIssueRepository issueRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
        }

        public async Task<bool> Handle(IssueAddCommand message, CancellationToken cancellationToken)
        {
            var issueEntity = new IssueEntity(
                message.Id,
                message.NexusModsGameId,
                message.NexusModsModId,
                message.Title,
                message.Url,
                message.ModVersion,
                message.Status,
                message.Priority,
                message.IsPrivate,
                message.IsClosed,
                message.IsDeleted,
                message.TimeOfLastPost);

            if (!(issueEntity.Content is null))
            {
                issueEntity.SetContent(
                    issueEntity.Content.Author,
                    issueEntity.Content.AuthorUrl,
                    issueEntity.Content.AvatarUrl,
                    issueEntity.Content.Content,
                    issueEntity.Content.IsDeleted,
                    issueEntity.Content.TimeOfPost);
            }

            foreach (var issueReply in message.Replies)
            {
                issueEntity.AddReplyEntity(
                    issueReply.Id,
                    issueReply.Author,
                    issueReply.AuthorUrl,
                    issueReply.AvatarUrl,
                    issueReply.Content,
                    issueReply.IsDeleted,
                    issueReply.TimeOfPost);
            }

            _issueRepository.Add(issueEntity);

            return await _issueRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}