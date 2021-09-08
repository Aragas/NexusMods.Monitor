namespace NexusMods.Monitor.Subscriptions.Application.Queries.Subscriptions
{
    public sealed record SubscriptionViewModel(string SubscriberId, uint NexusModsGameId, uint NexusModsModId, string NexusModsGameName, string NexusModsModName);
}