namespace NexusMods.Monitor.Bot.Discord.Application.Queries.Subscriptions
{
    public sealed record SubscriptionViewModel(ulong ChannelId, uint NexusModsGameId, uint NexusModsModId, string NexusModsGameName, string NexusModsModName);
}