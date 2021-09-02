namespace NexusMods.Monitor.Bot.Discord.Application.Queries
{
    public sealed record SubscriptionViewModel(ulong ChannelId, uint NexusModsGameId, uint NexusModsModId);
}