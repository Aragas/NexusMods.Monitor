namespace NexusMods.Monitor.Bot.Slack.Application.Queries
{
    public sealed record SubscriptionViewModel(string ChannelId, uint NexusModsGameId, uint NexusModsModId);
}