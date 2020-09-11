using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Slack.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using SlackNet.Bot;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.IntegrationEventHandlers.Issues
{
    public class IssueAddedNewIntegrationEventHandler : Enbiso.NLib.EventBus.EventHandler<IssueAddedIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly SlackBot _slackBot;

        public IssueAddedNewIntegrationEventHandler(ILogger<IssueAddedNewIntegrationEventHandler> logger,
            ISubscriptionRepository subscriptionRepository,
            SlackBot slackBot)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
            _slackBot = slackBot ?? throw new ArgumentNullException(nameof(slackBot));
        }

        protected override async Task Handle(IssueAddedIntegrationEvent command)
        {
            var embed = AttachmentHelper.NewIssue(command.Issue);

            foreach (var subscriptionEntity in await _subscriptionRepository.GetAllAsync().ToListAsync())
            {
                if (!(await _slackBot.GetConversationById(subscriptionEntity.ChannelId) is { } channel)) continue;
                if (subscriptionEntity.NexusModsGameId != command.Issue.NexusModsGameId || subscriptionEntity.NexusModsModId != command.Issue.NexusModsModId) continue;
                await _slackBot.Send(new BotMessage() {Conversation = new ConversationByRef(channel), Attachments = { embed }} );
            }
        }
    }
}