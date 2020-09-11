using Discord;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.IntegrationEventHandlers.Comments
{
    public class CommentAddedNewIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<CommentAddedIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly DiscordSocketClient _discordSocketClient;

        public CommentAddedNewIntegrationEventHandler(ILogger<CommentAddedNewIntegrationEventHandler> logger,
            ISubscriptionRepository subscriptionRepository,
            DiscordSocketClient discordSocketClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
            _discordSocketClient = discordSocketClient ?? throw new ArgumentNullException(nameof(discordSocketClient));
        }

        protected override async Task Handle(CommentAddedIntegrationEvent command)
        {
            //var commentEntity = await _commentRepository.GetAsync(command.Id);
            //if (commentEntity is null) throw new NullReferenceException();

            var embed = EmbedHelper.NewComment(command.Comment);

            foreach (var subscriptionEntity in await _subscriptionRepository.GetAllAsync().ToListAsync())
            {
                if (!(_discordSocketClient.GetChannel(subscriptionEntity.ChannelId) is IMessageChannel channel)) continue;
                if (subscriptionEntity.NexusModsGameId != command.Comment.NexusModsGameId || subscriptionEntity.NexusModsModId != command.Comment.NexusModsModId) continue;
                await channel.SendMessageAsync(embed: embed);
            }
        }
    }
}