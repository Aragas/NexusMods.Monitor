using System.Collections.Generic;
using System.Threading;

namespace NexusMods.Monitor.Scraper.Application.Queries.Comments
{
    public interface ICommentQueries
    {
        IAsyncEnumerable<CommentViewModel> GetAllAsync(CancellationToken ct = default);
    }
}