using NexusMods.Monitor.Shared.Domain.SeedWork;

using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate
{
    public interface ICommentRepository : IRepository<CommentEntity>
    {
        CommentEntity Add(CommentEntity commentEntity);

        CommentEntity Update(CommentEntity commentEntity);

        Task<CommentEntity?> GetAsync(uint commentEntityId);
        IQueryable<CommentEntity> GetAll();
    }
}