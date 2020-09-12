using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Application.Queries.Issues
{
    public interface IIssueQueries
    {
        IAsyncEnumerable<IssueViewModel> GetAllAsync();
    }
}