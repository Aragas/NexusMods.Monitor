using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public interface INexusModsCommentQueries
    {
        IAsyncEnumerable<NexusModsCommentRootViewModel> GetAllAsync(uint gameId, uint modId, CancellationToken ct = default);
        Task<bool> ExistsAsync(uint gameId, uint modId, uint commentId, CancellationToken ct);
        Task<bool> ExistsReplyAsync(uint gameId, uint modId, uint commentId, uint replyId, CancellationToken ct);
    }
}