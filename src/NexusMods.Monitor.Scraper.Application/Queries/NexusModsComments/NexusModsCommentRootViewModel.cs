namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public sealed record NexusModsCommentRootViewModel(string NexusModsGameIdText, uint NexusModsGameId, uint NexusModsModId, NexusModsCommentViewModel NexusModsComment);
}