using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public interface INexusModsIssueQueries
    {
        IAsyncEnumerable<NexusModsIssueRootViewModel> GetAllAsync(uint gameId, uint modId, CancellationToken ct = default);
        Task<NexusModsIssueContentViewModel?> GetContentAsync(uint issueId, CancellationToken ct = default);
        IAsyncEnumerable<NexusModsIssueReplyViewModel> GetRepliesAsync(uint issueId, CancellationToken ct = default);
    }
}