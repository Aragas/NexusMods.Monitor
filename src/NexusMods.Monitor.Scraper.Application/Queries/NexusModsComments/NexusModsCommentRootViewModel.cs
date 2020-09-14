using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    [DataContract]
    public sealed class NexusModsCommentRootViewModel
    {
        [DataMember]
        public string NexusModsGameIdText { get; private set; } = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsModId { get; private set; } = default!;
        [DataMember]
        public NexusModsCommentViewModel NexusModsComment { get; private set; } = default!;

        private NexusModsCommentRootViewModel() { }
        public NexusModsCommentRootViewModel(string nexusModsGameIdText, uint nexusModsGameId, uint nexusModsModId, NexusModsCommentViewModel nexusModsComment): this()
        {
            NexusModsGameIdText = nexusModsGameIdText;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            NexusModsComment = nexusModsComment;
        }
    }
}