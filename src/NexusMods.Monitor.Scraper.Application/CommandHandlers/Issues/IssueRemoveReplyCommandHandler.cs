using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public sealed class IssueRemoveReplyCommandHandler : IRequestHandler<IssueRemoveReplyCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;
        private readonly INexusModsIssueQueries _nexusModsIssueQueries;
        private readonly IIssueIntegrationEventPublisher _eventPublisher;

        public IssueRemoveReplyCommandHandler(ILogger<IssueRemoveReplyCommandHandler> logger, IIssueRepository issueRepository, INexusModsIssueQueries nexusModsIssueQueries, IIssueIntegrationEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
            _nexusModsIssueQueries = nexusModsIssueQueries ?? throw new ArgumentNullException(nameof(nexusModsIssueQueries));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(IssueRemoveReplyCommand message, CancellationToken ct)
        {
            if (await _issueRepository.GetAsync(message.Id) is not { } issueEntity)
            {
                _logger.LogError("Issue with Id {Id} was not found", message.Id);
                return false;
            }

            if (issueEntity.Replies.FirstOrDefault(r => r.Id == message.ReplyId) is not { } existingReplyEntity)
            {
                _logger.LogError("Issue with Id {Id} doesn't have the reply! IssueReply Id {ReplyId}", message.Id, message.ReplyId);
                return false;
            }

            if (await _nexusModsIssueQueries.ExistsReplyAsync(issueEntity.NexusModsGameId, issueEntity.NexusModsModId, existingReplyEntity.OwnerId, existingReplyEntity.Id, ct))
            {
                _logger.LogError("Issue Reply with Id {ReplyId} still exists in NexusMods!", message.ReplyId);
                return false;
            }

            var issueReply = issueEntity.RemoveReplyEntity(message.ReplyId)!;
            _issueRepository.Update(issueEntity);

            if (await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                var issueDTO = Mapper.Map(issueEntity);
                var issueReplyDTO = Mapper.Map(issueReply);
                await _eventPublisher.Publish(new IssueRemovedReplyIntegrationEvent(issueDTO, issueReplyDTO), ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}