using Enbiso.NLib.EventBus;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Host.Services
{
    public class IssueIntegrationEventPublisher : IIssueIntegrationEventPublisher
    {
        [SuppressMessage("CodeQuality", "IDE0052", Justification = "Reserved for future use")]
        private readonly ILogger _logger;
        private readonly IEventPublisher _eventPublisher;

        public IssueIntegrationEventPublisher(ILogger<IssueIntegrationEventPublisher> logger, IEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task Publish(IssueIntegrationEvent issueEvent, CancellationToken ct) => await _eventPublisher.Publish(issueEvent, "issue_events", ct);
    }
}