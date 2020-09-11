using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Subscriptions.Application.Commands;
using NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.Application.CommandHandlers
{
    public class SubscriptionRemoveCommandHandler : IRequestHandler<SubscriptionRemoveCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionRemoveCommandHandler(ILogger<SubscriptionRemoveCommandHandler> logger, ISubscriptionRepository subscriptionRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        }

        public async Task<bool> Handle(SubscriptionRemoveCommand message, CancellationToken cancellationToken)
        {
            var subscriptionEntity = new SubscriptionEntity(message.SubscriberId, message.NexusModsGameId, message.NexusModsModId);

            _subscriptionRepository.Remove(subscriptionEntity);

            return await _subscriptionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}