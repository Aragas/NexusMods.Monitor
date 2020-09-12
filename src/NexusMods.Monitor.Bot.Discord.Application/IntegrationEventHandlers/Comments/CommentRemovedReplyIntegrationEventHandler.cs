using Discord;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Application.Queries;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.IntegrationEventHandlers.Comments
{
    public class CommentRemovedReplyIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<CommentRemovedReplyIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionQueries _subscriptionQueries;
        private readonly IDiscordClient _discordClient;

        public CommentRemovedReplyIntegrationEventHandler(ILogger<CommentAddedNewIntegrationEventHandler> logger,
            ISubscriptionQueries subscriptionQueries,
            IDiscordClient discordClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
            _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
        }

        protected override async Task Handle(CommentRemovedReplyIntegrationEvent command)
        {
            var embed = EmbedHelper.DeletedCommentReply(command.Comment, command.DeletedCommentReply);

            foreach (var subscriptionEntity in await _subscriptionQueries.GetAllAsync().ToListAsync())
            {
                if (!(await _discordClient.GetChannelAsync(subscriptionEntity.ChannelId) is IMessageChannel channel)) continue;
                if (subscriptionEntity.NexusModsGameId != command.Comment.NexusModsGameId || subscriptionEntity.NexusModsModId != command.Comment.NexusModsModId) continue;
                await channel.SendMessageAsync(embed: embed);
            }
        }
    }
}