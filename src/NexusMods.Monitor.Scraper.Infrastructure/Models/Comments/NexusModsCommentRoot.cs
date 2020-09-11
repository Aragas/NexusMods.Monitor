namespace NexusMods.Monitor.Scraper.Infrastructure.Models.Comments
{
    public class NexusModsCommentRoot
    {
        public string NexusModsGameIdText { get; }
        public uint NexusModsGameId { get; }
        public uint NexusModsModId { get; }
        public NexusModsComment NexusModsComment { get; }

        public NexusModsCommentRoot(string nexusModsGameIdText, uint nexusModsGameId, uint nexusModsModId, NexusModsComment nexusModsComment)
        {
            NexusModsGameIdText = nexusModsGameIdText;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            NexusModsComment = nexusModsComment;
        }
    }
}