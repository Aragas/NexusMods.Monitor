using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Subscriptions.Application.Commands;
using NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.Application.CommandHandlers
{
    public sealed class SubscriptionAddCommandHandler : IRequestHandler<SubscriptionAddCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionAddCommandHandler(ILogger<SubscriptionAddCommandHandler> logger, ISubscriptionRepository subscriptionRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        }

        public async Task<bool> Handle(SubscriptionAddCommand message, CancellationToken ct)
        {
            var existingSubscription = await _subscriptionRepository.GetAsync(message.SubscriberId, message.NexusModsGameId, message.NexusModsModId);
            if (existingSubscription is { })
            {
                _logger.LogError("Subscription with Id {Id} already exists", message.SubscriberId);
                return false;
            }

            var subscriptionEntity = new SubscriptionEntity(message.SubscriberId, message.NexusModsGameId, message.NexusModsModId);
            _subscriptionRepository.Add(subscriptionEntity);

            return await _subscriptionRepository.UnitOfWork.SaveEntitiesAsync(ct);
        }
    }
}