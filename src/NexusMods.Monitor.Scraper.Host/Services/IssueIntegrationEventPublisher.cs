using Enbiso.NLib.EventBus;

using Microsoft.Extensions.Logging;

using NATS.Client;

using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using Polly;
using Polly.Extensions.Http;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
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

        public async Task Publish(IssueIntegrationEvent issueEvent, CancellationToken ct)
        {
            // TODO: Abstract NATSConnectionException
            await Policy
                .Handle<NATSConnectionException>()
                .WaitAndRetryAsync(
                    retryCount: 5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetryAsync: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogError(exception, "Exception during NATS connection. Retry count {RetryCount}. Waiting {Time}...", retryCount, timeSpan);
                        return Task.CompletedTask;
                    })
                .ExecuteAsync(async () =>
                {
                    await _eventPublisher.Publish(issueEvent, "issue_events", null, ct);
                });
        }
    }
}