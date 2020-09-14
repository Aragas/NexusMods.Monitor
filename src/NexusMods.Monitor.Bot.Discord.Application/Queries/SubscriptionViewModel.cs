using System.Runtime.Serialization;

namespace NexusMods.Monitor.Bot.Discord.Application.Queries
{
    [DataContract]
    public sealed class SubscriptionViewModel
    {
        [DataMember]
        public ulong ChannelId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
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