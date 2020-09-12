using System.Collections.Generic;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public interface INexusModsIssueQueries
    {
        IAsyncEnumerable<NexusModsIssueRootViewModel> GetAllAsync(uint gameId, uint modId);
        Task<NexusModsIssueContentViewModel?> GetContentAsync(uint issueId);
        IAsyncEnumerable<NexusModsIssueReplyViewModel> GetRepliesAsync(uint issueId);
    }
}