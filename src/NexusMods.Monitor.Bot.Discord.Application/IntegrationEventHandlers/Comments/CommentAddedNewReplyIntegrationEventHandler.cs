using Discord;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Application.Queries;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.IntegrationEventHandlers.Comments
{
    public sealed class CommentAddedNewReplyIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<CommentAddedReplyIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionQueries _subscriptionQueries;
        private readonly IDiscordClient _discordClient;

        public CommentAddedNewReplyIntegrationEventHandler(ILogger<CommentAddedNewReplyIntegrationEventHandler> logger,
            ISubscriptionQueries subscriptionQueries,
            IDiscordClient discordClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
            _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
        }

        protected override async Task Handle(CommentAddedReplyIntegrationEvent command)
        {
            var embed = EmbedHelper.NewCommentReply(command.Comment, command.Comment.Replies.First(x => x.Id == command.ReplyId));

            await foreach (var (channelId, nexusModsGameId, nexusModsModId) in _subscriptionQueries.GetAllAsync())
            {
                if (await _discordClient.GetChannelAsync(channelId) is not IMessageChannel channel) continue;
                if (nexusModsGameId != command.Comment.NexusModsGameId || nexusModsModId != command.Comment.NexusModsModId) continue;
                await channel.SendMessageAsync(embed: embed);
            }
        }
    }
}