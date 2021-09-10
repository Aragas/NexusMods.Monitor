﻿using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Slack.Application.Queries;
using NexusMods.Monitor.Bot.Slack.Application.Queries.Subscriptions;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using SlackNet.Bot;

using System;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.IntegrationEventHandlers.Issues
{
    public sealed class IssueChangedStatusIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<IssueChangedStatusIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionQueries _subscriptionQueries;
        private readonly ISlackBot _slackBot;

        public IssueChangedStatusIntegrationEventHandler(ILogger<IssueChangedStatusIntegrationEventHandler> logger, ISubscriptionQueries subscriptionQueries, ISlackBot slackBot)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
            _slackBot = slackBot ?? throw new ArgumentNullException(nameof(slackBot));
        }

        protected override async Task Handle(IssueChangedStatusIntegrationEvent command)
        {
            var embed = AttachmentHelper.StatusChanged(command.Issue, command.OldIssueStatus);

            await foreach (var (channelId, nexusModsGameId, nexusModsModId, _, _) in _subscriptionQueries.GetAllAsync())
            {
                if (await _slackBot.GetConversationById(channelId) is not { } channel) continue;
                if (nexusModsGameId != command.Issue.NexusModsGameId || nexusModsModId != command.Issue.NexusModsModId) continue;
                await _slackBot.Send(new BotMessage { Conversation = new ConversationByRef(channel), Attachments = { embed } });
            }
        }
    }
}