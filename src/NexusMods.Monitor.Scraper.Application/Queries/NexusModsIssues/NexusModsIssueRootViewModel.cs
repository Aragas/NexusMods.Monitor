using System.Collections.Generic;
using System.Collections.Immutable;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public sealed record NexusModsIssueRootViewModel(string GameDomain, uint GameId, uint ModId, string GameName, string ModName, NexusModsIssueViewModel Issue,
        NexusModsIssueContentViewModel? IssueContent, ImmutableArray<NexusModsIssueReplyViewModel> IssueReplies);
}