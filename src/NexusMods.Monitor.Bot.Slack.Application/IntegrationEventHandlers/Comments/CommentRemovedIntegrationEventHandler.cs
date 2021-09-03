using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Slack.Application.Queries;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using SlackNet.Bot;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.IntegrationEventHandlers.Comments
{
    public sealed class CommentRemovedIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<CommentRemovedIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionQueries _subscriptionQueries;
        private readonly ISlackBot _slackBot;

        public CommentRemovedIntegrationEventHandler(ILogger<CommentRemovedIntegrationEventHandler> logger, ISubscriptionQueries subscriptionQueries, ISlackBot slackBot)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
            _slackBot = slackBot ?? throw new ArgumentNullException(nameof(slackBot));
        }

        protected override async Task Handle(CommentRemovedIntegrationEvent command)
        {
            var embed = AttachmentHelper.DeletedComment(command.Comment);

            foreach (var (channelId, nexusModsGameId, nexusModsModId) in await _subscriptionQueries.GetAllAsync().ToListAsync())
            {
                if (await _slackBot.GetConversationById(channelId) is not { } channel) continue;
                if (nexusModsGameId != command.Comment.NexusModsGameId || nexusModsModId != command.Comment.NexusModsModId) continue;
                await _slackBot.Send(new BotMessage { Conversation = new ConversationByRef(channel), Attachments = { embed } });
            }
        }
    }
}