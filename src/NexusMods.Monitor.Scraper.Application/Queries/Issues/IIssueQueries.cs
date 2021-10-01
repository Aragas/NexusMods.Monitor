using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.Issues
{
    public interface IIssueQueries
    {
        IQueryable<IssueViewModel> GetAll();
        Task<IssueStatusEnumeration> GetStatusAsync(uint id, CancellationToken ct = default);
        Task<IssuePriorityEnumeration> GetPriorityAsync(uint id, CancellationToken ct = default);
    }
}