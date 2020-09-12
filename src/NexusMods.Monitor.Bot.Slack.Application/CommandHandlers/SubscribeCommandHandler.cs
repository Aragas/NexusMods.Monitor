using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Slack.Application.Commands;
using NexusMods.Monitor.Bot.Slack.Domain.AggregatesModel.SubscriptionAggregate;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.CommandHandlers
{
    public class SubscribeCommandHandler : IRequestHandler<SubscribeCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscribeCommandHandler(ILogger<SubscribeCommandHandler> logger, ISubscriptionRepository subscriptionRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        }

        public async Task<bool> Handle(SubscribeCommand message, CancellationToken cancellationToken)
        {
            await _subscriptionRepository.SubscribeAsync(message.ChannelId, message.NexusModsGameId, message.NexusModsModId);
            return await _subscriptionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}