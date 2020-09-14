using System.Runtime.Serialization;

namespace NexusMods.Monitor.Bot.Slack.Application.Queries
{
    [DataContract]
    public sealed class SubscriptionViewModel
    {
        [DataMember]
        public string ChannelId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
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