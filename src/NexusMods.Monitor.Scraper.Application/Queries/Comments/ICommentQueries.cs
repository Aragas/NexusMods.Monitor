using System.Linq;

namespace NexusMods.Monitor.Scraper.Application.Queries.Comments
{
    public interface ICommentQueries
    {
        IQueryable<CommentViewModel> GetAll();
    }
}