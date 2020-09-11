using System.Collections.Generic;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Infrastructure.Models.Issues
{
    public interface INexusModsIssuesRepository
    {
        IAsyncEnumerable<NexusModsIssueRoot> GetIssuesAsync(uint gameId, uint modId);
        Task<NexusModsIssueContent?> GetIssueContentAsync(uint issueId);
        IAsyncEnumerable<NexusModsIssueReply> GetIssueRepliesAsync(uint issueId);
    }
}