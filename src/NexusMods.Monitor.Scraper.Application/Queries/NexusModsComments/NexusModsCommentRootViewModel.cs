namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public class NexusModsCommentRootViewModel
    {
        public string NexusModsGameIdText { get; private set; } = default!;
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;
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