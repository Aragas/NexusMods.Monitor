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
        Task<bool> ExistsAsync(uint gameId, uint modId, uint issueId, CancellationToken ct);
        Task<bool> ExistsReplyAsync(uint gameId, uint modId, uint issueId, uint replyId, CancellationToken ct);
    }
}