using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Slack.Application.Queries;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using SlackNet.Bot;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.IntegrationEventHandlers.Comments
{
    public sealed class CommentAddedNewReplyIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<CommentAddedReplyIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionQueries _subscriptionQueries;
        private readonly ISlackBot _slackBot;

        public CommentAddedNewReplyIntegrationEventHandler(ILogger<CommentAddedNewReplyIntegrationEventHandler> logger, ISubscriptionQueries subscriptionQueries, ISlackBot slackBot)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
            _slackBot = slackBot ?? throw new ArgumentNullException(nameof(slackBot));
        }

        protected override async Task Handle(CommentAddedReplyIntegrationEvent command)
        {
            var embed = AttachmentHelper.NewCommentReply(command.Comment, command.Comment.Replies.First(x => x.Id == command.ReplyId));

            await foreach (var (channelId, nexusModsGameId, nexusModsModId) in _subscriptionQueries.GetAllAsync())
            {
                if (await _slackBot.GetConversationById(channelId) is not { } channel) continue;
                if (nexusModsGameId != command.Comment.NexusModsGameId || nexusModsModId != command.Comment.NexusModsModId) continue;
                await _slackBot.Send(new BotMessage { Conversation = new ConversationByRef(channel), Attachments = { embed } });
            }
        }
    }
}