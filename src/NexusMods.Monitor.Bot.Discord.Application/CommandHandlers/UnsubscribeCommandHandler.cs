using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Application.Commands;
using NexusMods.Monitor.Bot.Discord.Domain.AggregatesModel.SubscriptionAggregate;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.CommandHandlers
{
    public class UnsubscribeCommandHandler : IRequestHandler<UnsubscribeCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public UnsubscribeCommandHandler(ILogger<UnsubscribeCommandHandler> logger, ISubscriptionRepository subscriptionRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        }

        public async Task<bool> Handle(UnsubscribeCommand message, CancellationToken cancellationToken)
        {
            await _subscriptionRepository.RemoveAsync(new SubscriptionEntity(message.ChannelId, message.NexusModsGameId, message.NexusModsModId));
            return await _subscriptionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}