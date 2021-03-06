﻿using Discord;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Application.Queries;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.IntegrationEventHandlers.Issues
{
    public sealed class IssueChangedPriorityIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<IssueChangedPriorityIntegrationEvent>
    {
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
            var embed = EmbedHelper.PriorityChanged(command.Issue, command.OldIssuePriority);

            foreach (var subscriptionEntity in await _subscriptionQueries.GetAllAsync().ToListAsync())
            {
                if (!(await _discordClient.GetChannelAsync(subscriptionEntity.ChannelId) is IMessageChannel channel)) continue;
                if (subscriptionEntity.NexusModsGameId != command.Issue.NexusModsGameId || subscriptionEntity.NexusModsModId != command.Issue.NexusModsModId) continue;
                await channel.SendMessageAsync(embed: embed);
            }
        }
    }
}