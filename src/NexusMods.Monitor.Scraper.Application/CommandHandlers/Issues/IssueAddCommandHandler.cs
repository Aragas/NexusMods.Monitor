using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public sealed class IssueAddCommandHandler : IRequestHandler<IssueAddCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;

        public IssueAddCommandHandler(ILogger<IssueAddCommandHandler> logger, IIssueRepository issueRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
        }

        public async Task<bool> Handle(IssueAddCommand message, CancellationToken ct)
        {
            var existingIssueEntity = await _issueRepository.GetAsync(message.Id);
            if (existingIssueEntity is { })
            {
                if (existingIssueEntity.IsDeleted)
                {
                    existingIssueEntity.Return();
                    return await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct);
                }

                _logger.LogError("Issue with Id {Id} already exist, is not deleted.", message.Id);
                return false;
            }

            var issueEntity = new IssueEntity(
                message.Id,
                message.NexusModsGameId,
                message.NexusModsModId,
                message.GameName,
                message.ModName,
                message.Title,
                message.Url,
                message.ModVersion,
                await _issueRepository.GetStatusAsync(message.Status.Id),
                await _issueRepository.GetPriorityAsync(message.Priority.Id),
                message.IsPrivate,
                message.IsClosed,
                message.IsDeleted,
                message.TimeOfLastPost);

            if (message.Content is { })
            {
                issueEntity.SetContent(
                    message.Content.Author,
                    message.Content.AuthorUrl,
                    message.Content.AvatarUrl,
                    message.Content.Content,
                    message.Content.IsDeleted,
                    message.Content.TimeOfPost);
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

            return await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct);
        }
    }
}