namespace NexusMods.Monitor.Subscriptions.Application.Queries
{
    public class SubscriptionViewModel
    {
        public string SubscriberId { get; private set; } = default!;
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;

        private SubscriptionViewModel() { }
        public SubscriptionViewModel(string subscriberId, uint nexusModsGameId, uint nexusModsModId)
        {
            SubscriberId = subscriberId;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
        }
    }
}