using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Slack.Application.Queries;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using SlackNet.Bot;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.IntegrationEventHandlers.Issues
{
    public sealed class IssueChangedIsPrivateIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<IssueChangedIsPrivateIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionQueries _subscriptionQueries;
        private readonly ISlackBot _slackBot;

        public IssueChangedIsPrivateIntegrationEventHandler(ILogger<IssueChangedIsPrivateIntegrationEventHandler> logger, ISubscriptionQueries subscriptionQueries, ISlackBot slackBot)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
            _slackBot = slackBot ?? throw new ArgumentNullException(nameof(slackBot));
        }

        protected override async Task Handle(IssueChangedIsPrivateIntegrationEvent command)
        {
            var embed = AttachmentHelper.IsPrivateChanged(command.Issue);

            foreach (var (channelId, nexusModsGameId, nexusModsModId) in await _subscriptionQueries.GetAllAsync().ToListAsync())
            {
                if (await _slackBot.GetConversationById(channelId) is not { } channel) continue;
                if (nexusModsGameId != command.Issue.NexusModsGameId || nexusModsModId != command.Issue.NexusModsModId) continue;
                await _slackBot.Send(new BotMessage { Conversation = new ConversationByRef(channel), Attachments = { embed } });
            }
        }
    }
}