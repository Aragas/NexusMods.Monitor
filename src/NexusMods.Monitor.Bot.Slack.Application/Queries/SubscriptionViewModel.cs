namespace NexusMods.Monitor.Bot.Slack.Application.Queries
{
    public class SubscriptionViewModel
    {
        public string ChannelId { get; private set; } = default!;
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;

        private SubscriptionViewModel() { }
        public SubscriptionViewModel(string channelId, uint nexusModsGameId, uint nexusModsModId)
        {
            ChannelId = channelId;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
        }
    }
}