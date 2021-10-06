using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.Issues
{
    public interface IIssueQueries
    {
        IAsyncEnumerable<IssueViewModel> GetAllAsync(uint gameId, uint modId, CancellationToken ct = default);
        Task<bool> ExistsAsync(uint gameId, uint modId, uint issueId, CancellationToken ct = default);
        Task<bool> ExistsReplyAsync(uint gameId, uint modId, uint issueId, uint replyId, CancellationToken ct = default);
        Task<IssueContentViewModel?> GetContentAsync(uint issueId, CancellationToken ct = default);
        IAsyncEnumerable<IssueReplyViewModel> GetRepliesAsync(uint issueId, CancellationToken ct = default);
    }
}