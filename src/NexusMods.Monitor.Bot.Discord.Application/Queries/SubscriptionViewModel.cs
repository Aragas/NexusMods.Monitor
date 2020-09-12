namespace NexusMods.Monitor.Bot.Discord.Application.Queries
{
    public sealed class SubscriptionViewModel
    {
        public ulong ChannelId { get; private set; } = default!;
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;

        private SubscriptionViewModel() { }
        public SubscriptionViewModel(ulong channelId, uint nexusModsGameId, uint nexusModsModId)
        {
            ChannelId = channelId;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
        }
    }
}