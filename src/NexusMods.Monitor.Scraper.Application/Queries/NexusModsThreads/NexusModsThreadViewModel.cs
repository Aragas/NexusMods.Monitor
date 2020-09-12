namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsThreads
{
    public sealed class NexusModsThreadViewModel
    {
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;
        public uint ThreadId { get; private set; } = default!;

        private NexusModsThreadViewModel() { }
        public NexusModsThreadViewModel(uint nexusModsGameId, uint nexusModsModId, uint threadId)
        {
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            ThreadId = threadId;
        }
    }
}