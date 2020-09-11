using AutoMapper;

using Enbiso.NLib.EventBus;

using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Application;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public class IssueChangePriorityCommandHandler : IRequestHandler<IssueChangePriorityCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public IssueChangePriorityCommandHandler(ILogger<IssueChangePriorityCommandHandler> logger, IIssueRepository issueRepository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(IssueChangePriorityCommand message, CancellationToken cancellationToken)
        {
            var issueEntity = await _issueRepository.GetAsync(message.Id);
            if (issueEntity is null) return false;

            var oldPriority = _mapper.Map<IssuePriorityEnumeration, IssueDTO.IssuePriorityDTO>(issueEntity.Priority);
            issueEntity.SetPriority(IssuePriorityEnumeration.From(message.PriorityId));

            _issueRepository.Update(issueEntity);

            var issueDTO = _mapper.Map<IssueEntity, IssueDTO>(issueEntity);

            if (await _issueRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                await _eventPublisher.Publish(new IssueChangedPriorityIntegrationEvent(issueDTO, oldPriority), "issue_events", cancellationToken);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}