using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Subscriptions.Application.Commands;
using NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.Application.CommandHandlers
{
    public sealed class SubscriptionRemoveCommandHandler : IRequestHandler<SubscriptionRemoveCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionRemoveCommandHandler(ILogger<SubscriptionRemoveCommandHandler> logger, ISubscriptionRepository subscriptionRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        }

        public async Task<bool> Handle(SubscriptionRemoveCommand message, CancellationToken ct)
        {
            var existingSubscription = await _subscriptionRepository.GetAsync(message.SubscriberId, message.NexusModsGameId, message.NexusModsModId);
            if (existingSubscription is null)
            {
                _logger.LogError("Subscription with Id {Id} does not exist", message.SubscriberId);
                return false;
            }

            _subscriptionRepository.Remove(existingSubscription);

            return await _subscriptionRepository.UnitOfWork.SaveEntitiesAsync(ct);
        }
    }
}