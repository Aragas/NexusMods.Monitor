﻿using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public sealed class IssueRemoveCommandHandler : IRequestHandler<IssueRemoveCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;
        private readonly INexusModsIssueQueries _nexusModsIssueQueries;
        private readonly IIssueIntegrationEventPublisher _eventPublisher;

        public IssueRemoveCommandHandler(ILogger<IssueRemoveCommandHandler> logger, IIssueRepository issueRepository, INexusModsIssueQueries nexusModsIssueQueries, IIssueIntegrationEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
            _nexusModsIssueQueries = nexusModsIssueQueries ?? throw new ArgumentNullException(nameof(nexusModsIssueQueries));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(IssueRemoveCommand message, CancellationToken ct)
        {
            if (await _issueRepository.GetAsync(message.Id) is not { } issueEntity)
            {
                _logger.LogError("Issue with Id {Id} was not found", message.Id);
                return false;
            }

            if (await _nexusModsIssueQueries.ExistsAsync(issueEntity.NexusModsGameId, issueEntity.NexusModsModId, issueEntity.Id, ct))
            {
                _logger.LogError("Issue with Id {Id} still exists in NexusMods!", message.Id);
                return false;
            }

            issueEntity.Remove();
            _issueRepository.Update(issueEntity);

            if (await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                var issueDTO = Mapper.Map(issueEntity);
                await _eventPublisher.Publish(new IssueRemovedIntegrationEvent(issueDTO), ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}