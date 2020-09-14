using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsThreads
{
    [DataContract]
    public sealed class NexusModsThreadViewModel
    {
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsModId { get; private set; } = default!;
        [DataMember]
        public uint ThreadId { get; private set; } = default!;

        private NexusModsThreadViewModel() { }
        public NexusModsThreadViewModel(uint nexusModsGameId, uint nexusModsModId, uint threadId) : this()
        {
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            ThreadId = threadId;
        }
    }
}