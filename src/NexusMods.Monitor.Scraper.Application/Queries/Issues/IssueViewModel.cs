using NodaTime;

using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Application.Queries.Issues
{
    public sealed record IssueViewModel(uint Id, uint NexusModsGameId, uint NexusModsModId, int Status, int Priority, bool IsClosed, bool IsPrivate, Instant TimeOfLastPost, IReadOnlyList<IssueReplyViewModel> Replies);
}