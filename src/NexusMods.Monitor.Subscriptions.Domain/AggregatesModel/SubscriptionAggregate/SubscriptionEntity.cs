using NexusMods.Monitor.Shared.Domain.SeedWork;

namespace NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate
{
    public sealed class SubscriptionEntity : IAggregateRoot
    {
        public string SubscriberId { get; private set; } = default!;
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;

        private SubscriptionEntity() { }
        public SubscriptionEntity(string subscriberId, uint nexusModsGameId, uint nexusModsModId)
        {
            SubscriberId = subscriberId;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
        }
    }
}