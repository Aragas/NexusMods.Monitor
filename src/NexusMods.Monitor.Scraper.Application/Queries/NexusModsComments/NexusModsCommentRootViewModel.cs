namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public sealed record NexusModsCommentRootViewModel(string GameDomain, uint GameId, uint ModId, string GameName, string ModName, NexusModsCommentViewModel NexusModsComment);
}