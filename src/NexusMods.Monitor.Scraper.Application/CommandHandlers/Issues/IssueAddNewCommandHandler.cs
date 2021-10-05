using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public sealed class IssueAddNewCommandHandler : IRequestHandler<IssueAddNewCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;
        private readonly IIssueIntegrationEventPublisher _eventPublisher;

        public IssueAddNewCommandHandler(ILogger<IssueAddNewCommandHandler> logger, IIssueRepository issueRepository, IIssueIntegrationEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(IssueAddNewCommand message, CancellationToken ct)
        {
            if (await _issueRepository.GetAsync(message.Id) is { } existingIssueEntity)
            {
                if (existingIssueEntity.IsDeleted)
                {
                    existingIssueEntity.Return();
                    return await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct);
                }

                _logger.LogError("Issue with Id {Id} already exist, is not deleted. Existing: {@ExistingIssue}, new: {Message}", message.Id, existingIssueEntity, message);
                return false;
            }

            var issueEntity = Mapper.Map(message, await _issueRepository.GetStatusAsync(message.StatusId), await _issueRepository.GetPriorityAsync(message.PriorityId));
            _issueRepository.Add(issueEntity);

            if (await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                var issueDTO = Mapper.Map(issueEntity);
                await _eventPublisher.Publish(new IssueAddedIntegrationEvent(issueDTO), ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}