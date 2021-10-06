using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.Comments
{
    public interface ICommentQueries
    {
        IAsyncEnumerable<CommentViewModel> GetAllAsync(uint gameId, uint modId, CancellationToken ct = default);
        Task<bool> ExistsAsync(uint gameId, uint modId, uint commentId, CancellationToken ct = default);
        Task<bool> ExistsReplyAsync(uint gameId, uint modId, uint commentId, uint replyId, CancellationToken ct = default);
    }
}