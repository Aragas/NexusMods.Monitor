using Enbiso.NLib.EventBus;

using Microsoft.Extensions.Logging;

using NATS.Client;

using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using Polly;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Host.Services
{
    public class CommentIntegrationEventPublisher : ICommentIntegrationEventPublisher
    {
        [SuppressMessage("CodeQuality", "IDE0052", Justification = "Reserved for future use")]
        private readonly ILogger _logger;
        private readonly IEventPublisher _eventPublisher;

        public CommentIntegrationEventPublisher(ILogger<CommentIntegrationEventPublisher> logger, IEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task Publish(CommentIntegrationEvent commentEvent, CancellationToken ct)
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
                    await _eventPublisher.Publish(commentEvent, "comment_events", null, ct);
                });
        }
    }
}