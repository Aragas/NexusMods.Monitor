using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Slack.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using SlackNet.Bot;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.IntegrationEventHandlers.Comments
{
    public class CommentAddedNewReplyIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<CommentAddedReplyIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly SlackBot _slackBot;

        public CommentAddedNewReplyIntegrationEventHandler(ILogger<CommentAddedNewReplyIntegrationEventHandler> logger,
            ISubscriptionRepository subscriptionRepository,
            SlackBot slackBot)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
            _slackBot = slackBot ?? throw new ArgumentNullException(nameof(slackBot));
        }

        protected override async Task Handle(CommentAddedReplyIntegrationEvent command)
        {
            var embed = AttachmentHelper.NewCommentReply(command.Comment, command.Comment.Replies.First(x => x.Id == command.ReplyId));

            foreach (var subscriptionEntity in await _subscriptionRepository.GetAllAsync().ToListAsync())
            {
                if (!(await _slackBot.GetConversationById(subscriptionEntity.ChannelId) is { } channel)) continue;
                if (subscriptionEntity.NexusModsGameId != command.Comment.NexusModsGameId || subscriptionEntity.NexusModsModId != command.Comment.NexusModsModId) continue;
                await _slackBot.Send(new BotMessage() {Conversation = new ConversationByRef(channel), Attachments = { embed }} );
            }
        }
    }
}