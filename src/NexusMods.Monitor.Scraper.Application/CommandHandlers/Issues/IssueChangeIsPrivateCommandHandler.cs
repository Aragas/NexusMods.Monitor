﻿using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public sealed class IssueChangeIsPrivateCommandHandler : IRequestHandler<IssueChangeIsPrivateCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;
        private readonly IIssueIntegrationEventPublisher _eventPublisher;

        public IssueChangeIsPrivateCommandHandler(ILogger<IssueChangeIsPrivateCommandHandler> logger, IIssueRepository issueRepository, IIssueIntegrationEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(IssueChangeIsPrivateCommand message, CancellationToken ct)
        {
            if (await _issueRepository.GetAsync(message.Id) is not { } issueEntity)
            {
                _logger.LogError("Issue with Id {Id} was not found", message.Id);
                return false;
            }

            if (issueEntity.IsPrivate != message.IsPrivate)
            {
                _logger.LogError("Issue with Id {Id} has already the correct IsPrivate value", message.Id);
                return false;
            }

            var oldIsPrivate = issueEntity.IsPrivate;
            issueEntity.SetIsPrivate(message.IsPrivate);
            _issueRepository.Update(issueEntity);

            if (await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                var issueDTO = Mapper.Map(issueEntity);
                await _eventPublisher.Publish(new IssueChangedIsPrivateIntegrationEvent(issueDTO, oldIsPrivate), ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}