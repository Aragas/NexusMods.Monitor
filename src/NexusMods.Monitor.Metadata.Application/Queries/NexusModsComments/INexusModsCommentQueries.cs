using System.Collections.Generic;
using System.Threading;

namespace NexusMods.Monitor.Metadata.Application.Queries.NexusModsComments
{
    public interface INexusModsCommentQueries
    {
        IAsyncEnumerable<NexusModsCommentRootViewModel> GetAllAsync(uint gameId, uint modId, CancellationToken ct = default);
    }
}