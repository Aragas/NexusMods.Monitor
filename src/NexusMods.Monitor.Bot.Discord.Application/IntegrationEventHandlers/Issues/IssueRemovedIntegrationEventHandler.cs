using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.IntegrationEventHandlers.Issues
{
    public class IssueRemovedIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<IssueRemovedIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly DiscordSocketClient _discordSocketClient;

        public IssueRemovedIntegrationEventHandler(ILogger<IssueAddedNewIntegrationEventHandler> logger,
            ISubscriptionRepository subscriptionRepository,
            DiscordSocketClient discordSocketClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
            _discordSocketClient = discordSocketClient ?? throw new ArgumentNullException(nameof(discordSocketClient));
        }

        protected override async Task Handle(IssueRemovedIntegrationEvent command)
        {
            var embed = EmbedHelper.DeletedIssue(command.Issue);

            foreach (var subscriptionEntity in await _subscriptionRepository.GetAllAsync().ToListAsync())
            {
                if (!(_discordSocketClient.GetChannel(subscriptionEntity.ChannelId) is IMessageChannel channel)) continue;
                if (subscriptionEntity.NexusModsGameId != command.Issue.NexusModsGameId || subscriptionEntity.NexusModsModId != command.Issue.NexusModsModId) continue;
                await channel.SendMessageAsync(embed: embed);
            }
        }
    }
}