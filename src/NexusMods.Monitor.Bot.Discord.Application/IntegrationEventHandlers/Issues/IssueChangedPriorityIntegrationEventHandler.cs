using Discord;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Application.Queries.Subscriptions;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.IntegrationEventHandlers.Issues
{
    public sealed class IssueChangedPriorityIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<IssueChangedPriorityIntegrationEvent>
    {
        [SuppressMessage("CodeQuality", "IDE0052", Justification = "Reserved for future use")]
        private readonly ILogger _logger;
        private readonly ISubscriptionQueries _subscriptionQueries;
        private readonly IDiscordClient _discordClient;

        public IssueChangedPriorityIntegrationEventHandler(ILogger<IssueChangedPriorityIntegrationEventHandler> logger,
            ISubscriptionQueries subscriptionQueries,
            IDiscordClient discordClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
            _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
        }

        protected override async Task Handle(IssueChangedPriorityIntegrationEvent command)
        {
            var embed = EmbedHelper.PriorityChanged(command.Issue, command.PreviousPriority);

            await foreach (var (channelId, nexusModsGameId, nexusModsModId, _, _) in _subscriptionQueries.GetAllAsync())
            {
                if (await _discordClient.GetChannelAsync(channelId) is not IMessageChannel channel) continue;
                if (nexusModsGameId != command.Issue.NexusModsGameId || nexusModsModId != command.Issue.NexusModsModId) continue;
                await channel.SendMessageAsync(embed: embed);
            }
        }
    }
}