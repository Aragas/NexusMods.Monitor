using NodaTime;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public sealed record NexusModsIssueViewModel(uint Id, string Title, bool IsPrivate, bool IsClosed, NexusModsIssueStatus Status, uint ReplyCount, string ModVersion, NexusModsIssuePriority Priority, Instant LastPost);
}