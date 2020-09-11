using NexusMods.Monitor.Shared.Domain.SeedWork;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.SubscriptionAggregate
{
    public sealed class SubscriptionEntity : IAggregateRoot
    {
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;

        private SubscriptionEntity() { }
        public SubscriptionEntity(uint nexusModsGameId, uint nexusModsModId)
        {
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
        }
    }
}