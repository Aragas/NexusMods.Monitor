using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Application.Queries.Comments
{
    public interface ICommentQueries
    {
        IAsyncEnumerable<CommentViewModel> GetAllAsync();
    }
}