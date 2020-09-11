using NexusMods.Monitor.Shared.Domain.SeedWork;

namespace NexusMods.Monitor.Bot.Slack.Domain.AggregatesModel.SubscriptionAggregate
{
    public sealed class SubscriptionEntity : IAggregateRoot
    {
        public string ChannelId { get; private set; } = default!;
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;

        private SubscriptionEntity() { }
        public SubscriptionEntity(string channelId, uint nexusModsGameId, uint nexusModsModId)
        {
            ChannelId = channelId;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
        }
    }
}