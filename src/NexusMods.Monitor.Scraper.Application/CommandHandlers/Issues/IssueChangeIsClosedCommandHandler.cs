using AutoMapper;

using Enbiso.NLib.EventBus;

using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;
using NexusMods.Monitor.Shared.Application.Models;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public sealed class IssueChangeIsClosedCommandHandler : IRequestHandler<IssueChangeIsClosedCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public IssueChangeIsClosedCommandHandler(ILogger<IssueChangeIsClosedCommandHandler> logger, IIssueRepository issueRepository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(IssueChangeIsClosedCommand message, CancellationToken ct)
        {
            var issueEntity = await _issueRepository.GetAsync(message.Id);
            if (issueEntity is null)
            {
                _logger.LogError("Issue with Id {Id} was not found.", message.Id);
                return false;
            }

            if (issueEntity.IsClosed != message.IsClosed)
            {
                _logger.LogError("Issue with Id {Id} has already the correct IsClosed value.", message.Id);
                return false;
            }

            var oldIsClosed = issueEntity.IsClosed;
            issueEntity.SetIsClosed(message.IsClosed);

            _issueRepository.Update(issueEntity);

            var issueDTO = _mapper.Map<IssueEntity, IssueDTO>(issueEntity);

            if (await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                await _eventPublisher.Publish(new IssueChangedIsClosedIntegrationEvent(issueDTO, oldIsClosed), "issue_events", ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}