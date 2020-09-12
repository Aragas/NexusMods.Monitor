namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public class NexusModsCommentRootViewModel
    {
        public string NexusModsGameIdText { get; }
        public uint NexusModsGameId { get; }
        public uint NexusModsModId { get; }
        public NexusModsCommentViewModel NexusModsComment { get; }

        public NexusModsCommentRootViewModel(string nexusModsGameIdText, uint nexusModsGameId, uint nexusModsModId, NexusModsCommentViewModel nexusModsComment)
        {
            NexusModsGameIdText = nexusModsGameIdText;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            NexusModsComment = nexusModsComment;
        }
    }
}