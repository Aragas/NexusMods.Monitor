using NexusMods.Monitor.Shared.Domain.SeedWork;

namespace NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate
{
    public sealed record SubscriptionEntity(string SubscriberId, uint NexusModsGameId, uint NexusModsModId) : IAggregateRoot;
}