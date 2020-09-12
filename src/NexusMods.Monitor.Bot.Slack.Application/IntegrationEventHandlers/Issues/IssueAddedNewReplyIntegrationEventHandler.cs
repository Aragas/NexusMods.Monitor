using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Slack.Application.Queries;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using SlackNet.Bot;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.IntegrationEventHandlers.Issues
{
    public class IssueAddedNewReplyIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<IssueAddedReplyIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionQueries _subscriptionQueries;
        private readonly ISlackBot _slackBot;

        public IssueAddedNewReplyIntegrationEventHandler(ILogger<IssueChangedStatusIntegrationEventHandler> logger,
            ISubscriptionQueries subscriptionQueries,
            ISlackBot slackBot)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
            _slackBot = slackBot ?? throw new ArgumentNullException(nameof(slackBot));
        }

        protected override async Task Handle(IssueAddedReplyIntegrationEvent command)
        {
            var embed = AttachmentHelper.NewIssueReply(command.Issue, command.Issue.Replies.First(x => x.Id == command.ReplyId));

            foreach (var subscriptionEntity in await _subscriptionQueries.GetAllAsync().ToListAsync())
            {
                if (!(await _slackBot.GetConversationById(subscriptionEntity.ChannelId) is { } channel)) continue;
                if (subscriptionEntity.NexusModsGameId != command.Issue.NexusModsGameId || subscriptionEntity.NexusModsModId != command.Issue.NexusModsModId) continue;
                await _slackBot.Send(new BotMessage {Conversation = new ConversationByRef(channel), Attachments = { embed }} );
            }
        }
    }
}