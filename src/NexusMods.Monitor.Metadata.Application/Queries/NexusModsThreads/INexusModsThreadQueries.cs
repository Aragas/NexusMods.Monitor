using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.NexusModsThreads
{
    public interface INexusModsThreadQueries
    {
        Task<NexusModsThreadViewModel> GetAsync(uint gameId, uint modId, CancellationToken ct = default);
    }
}