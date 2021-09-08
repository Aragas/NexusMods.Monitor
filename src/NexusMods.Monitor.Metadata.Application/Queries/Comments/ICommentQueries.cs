using System.Collections.Generic;
using System.Threading;

namespace NexusMods.Monitor.Metadata.Application.Queries.Comments
{
    public interface ICommentQueries
    {
        IAsyncEnumerable<CommentViewModel> GetAllAsync(uint gameId, uint modId, CancellationToken ct = default);
    }
}