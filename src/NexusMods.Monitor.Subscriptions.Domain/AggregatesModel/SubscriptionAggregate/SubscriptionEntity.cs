using NexusMods.Monitor.Shared.Domain.SeedWork;
using NexusMods.Monitor.Subscriptions.Domain.ValueObject;

namespace NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate
{
    public sealed record SubscriptionEntity(SubscriptionId Id) : Entity<SubscriptionId>(Id), IAggregateRoot
    {
        public string SubscriberId { get => Id.SubscriberId; set => Id.SubscriberId = value; }
        public uint NexusModsGameId { get => Id.NexusModsGameId; set => Id.NexusModsGameId = value; }
        public uint NexusModsModId { get => Id.NexusModsModId; set => Id.NexusModsModId = value; }

        public SubscriptionEntity(string subscriberId, uint nexusModsGameId, uint nexusModsModId) : this(new SubscriptionId(subscriberId, nexusModsGameId, nexusModsModId)) { }
    }
}