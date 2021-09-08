using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.Application.Queries.NexusModsMods
{
    public interface INexusModsModQueries
    {
        Task<NexusModsModViewModel?> GetAsync(uint gameId, uint modId, CancellationToken ct = default);
    }
}