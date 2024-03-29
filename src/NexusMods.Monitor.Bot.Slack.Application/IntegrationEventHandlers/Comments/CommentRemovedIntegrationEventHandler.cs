﻿using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Slack.Application.Queries.Subscriptions;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments;

using SlackNet.Bot;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.IntegrationEventHandlers.Comments
{
    public sealed class CommentRemovedIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<CommentRemovedIntegrationEvent>
    {
        [SuppressMessage("CodeQuality", "IDE0052", Justification = "Reserved for future use")]
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

            await foreach (var (channelId, nexusModsGameId, nexusModsModId, _, _) in _subscriptionQueries.GetAllAsync())
            {
                if (await _slackBot.GetConversationById(channelId) is not { } channel) continue;
                if (nexusModsGameId != command.Comment.NexusModsGameId || nexusModsModId != command.Comment.NexusModsModId) continue;
                await _slackBot.Send(new BotMessage { Conversation = new ConversationByRef(channel), Attachments = { embed } });
            }
        }
    }
}