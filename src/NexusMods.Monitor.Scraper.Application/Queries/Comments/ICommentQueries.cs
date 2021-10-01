using System.Collections.Generic;
using System.Threading;

namespace NexusMods.Monitor.Scraper.Application.Queries.Comments
{
    public interface ICommentQueries
    {
        IAsyncEnumerable<CommentViewModel> GetAllAsync(uint nexusModsGameId, uint nexusModsModId, CancellationToken ct);
        IAsyncEnumerable<CommentViewModel> GetAllAsync(CancellationToken ct);
    }
}