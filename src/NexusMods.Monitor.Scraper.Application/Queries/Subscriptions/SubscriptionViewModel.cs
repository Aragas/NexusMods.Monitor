namespace NexusMods.Monitor.Scraper.Application.Queries.Subscriptions
{
    public sealed class SubscriptionViewModel
    {
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;

        private SubscriptionViewModel() { }
        public SubscriptionViewModel(uint nexusModsGameId, uint nexusModsModId) : this()
        {
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
        }
    }
}