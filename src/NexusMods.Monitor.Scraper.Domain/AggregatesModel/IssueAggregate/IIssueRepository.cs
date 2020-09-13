using NexusMods.Monitor.Shared.Domain.SeedWork;

using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    public interface IIssueRepository : IRepository<IssueEntity>
    {
        IssueEntity Add(IssueEntity issueEntity);

        IssueEntity Update(IssueEntity issueEntity);

        Task<IssueEntity?> GetAsync(uint issueEntityId);
        Task<IssueStatusEnumeration> GetStatusAsync(int issueStatusEnumerationId);
        Task<IssuePriorityEnumeration> GetPriorityAsync(int issuePriorityEnumerationId);
        IQueryable<IssueEntity> GetAll();
    }
}