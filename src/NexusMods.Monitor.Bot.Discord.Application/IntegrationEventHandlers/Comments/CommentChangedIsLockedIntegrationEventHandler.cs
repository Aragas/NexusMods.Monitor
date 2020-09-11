using Discord;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.IntegrationEventHandlers.Comments
{
    public class CommentChangedIsLockedIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<CommentChangedIsLockedIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IDiscordClient _discordClient;

        public CommentChangedIsLockedIntegrationEventHandler(ILogger<CommentChangedIsLockedIntegrationEventHandler> logger,
            ISubscriptionRepository subscriptionRepository,
            IDiscordClient discordClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
            _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
        }

        protected override async Task Handle(CommentChangedIsLockedIntegrationEvent command)
        {
            var embed = EmbedHelper.IsLockedChanged(command.Comment);

            foreach (var subscriptionEntity in await _subscriptionRepository.GetAllAsync().ToListAsync())
            {
                if (!(await _discordClient.GetChannelAsync(subscriptionEntity.ChannelId) is IMessageChannel channel)) continue;
                if (subscriptionEntity.NexusModsGameId != command.Comment.NexusModsGameId || subscriptionEntity.NexusModsModId != command.Comment.NexusModsModId) continue;
                await channel.SendMessageAsync(embed: embed);
            }
        }
    }
}