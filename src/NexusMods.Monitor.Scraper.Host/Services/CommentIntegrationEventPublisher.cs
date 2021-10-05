using Enbiso.NLib.EventBus;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

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

        public async Task Publish(CommentIntegrationEvent commentEvent, CancellationToken ct) => await _eventPublisher.Publish(commentEvent, "comment_events", ct);
    }
}