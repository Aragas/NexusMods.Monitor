using NexusMods.Monitor.Shared.Domain.SeedWork;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsThreadAggregate
{
    public sealed class NexusModsThreadEntity : IAggregateRoot
    {
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;
        public uint ThreadId { get; private set; } = default!;

        private NexusModsThreadEntity() { }
        public NexusModsThreadEntity(uint nexusModsGameId, uint nexusModsModId, uint threadId)
        {
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            ThreadId = threadId;
        }
    }
}