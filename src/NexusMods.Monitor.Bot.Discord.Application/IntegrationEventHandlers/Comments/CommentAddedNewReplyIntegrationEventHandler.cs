using Discord;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.IntegrationEventHandlers.Comments
{
    public class CommentAddedNewReplyIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<CommentAddedReplyIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IDiscordClient _discordClient;

        public CommentAddedNewReplyIntegrationEventHandler(ILogger<CommentAddedNewReplyIntegrationEventHandler> logger,
            ISubscriptionRepository subscriptionRepository,
            IDiscordClient discordClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
            _discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
        }

        protected override async Task Handle(CommentAddedReplyIntegrationEvent command)
        {
            var embed = EmbedHelper.NewCommentReply(command.Comment, command.Comment.Replies.First(x => x.Id == command.ReplyId));

            foreach (var subscriptionEntity in await _subscriptionRepository.GetAllAsync().ToListAsync())
            {
                if (!(await _discordClient.GetChannelAsync(subscriptionEntity.ChannelId) is IMessageChannel channel)) continue;
                if (subscriptionEntity.NexusModsGameId != command.Comment.NexusModsGameId || subscriptionEntity.NexusModsModId != command.Comment.NexusModsModId) continue;
                await channel.SendMessageAsync(embed: embed);
            }
        }
    }
}