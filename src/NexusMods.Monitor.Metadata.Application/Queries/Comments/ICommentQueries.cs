using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.Comments
{
    public interface ICommentQueries
    {
        IAsyncEnumerable<CommentViewModel> GetAllAsync(uint gameId, uint modId, CancellationToken ct = default);
        Task<CommentViewModel?> GetAsync(uint gameId, uint modId, uint commentId, CancellationToken ct = default);
    }
}