namespace NexusMods.Monitor.Bot.Slack.Application.Queries.Subscriptions
{
    public sealed record SubscriptionViewModel(string ChannelId, uint NexusModsGameId, uint NexusModsModId, string NexusModsGameName, string NexusModsModName);
}