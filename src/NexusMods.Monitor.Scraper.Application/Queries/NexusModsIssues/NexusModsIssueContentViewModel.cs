using NodaTime;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public sealed record NexusModsIssueContentViewModel(uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant Time);
}