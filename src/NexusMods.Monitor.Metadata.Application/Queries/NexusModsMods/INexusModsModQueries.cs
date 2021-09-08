using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.NexusModsMods
{
    public interface INexusModsModQueries
    {
        Task<NexusModsModViewModel?> GetAsync(uint gameId, uint modId, CancellationToken ct = default);
        Task<NexusModsModViewModel?> GetAsync(string gameDomain, uint modId, CancellationToken ct = default);
    }
}