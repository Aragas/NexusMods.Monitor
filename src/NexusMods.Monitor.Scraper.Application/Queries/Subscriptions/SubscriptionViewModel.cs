using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Queries.Subscriptions
{
    [DataContract]
    public sealed class SubscriptionViewModel
    {
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsModId { get; private set; } = default!;

        private SubscriptionViewModel() { }
        public SubscriptionViewModel(uint nexusModsGameId, uint nexusModsModId) : this()
        {
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
        }
    }
}