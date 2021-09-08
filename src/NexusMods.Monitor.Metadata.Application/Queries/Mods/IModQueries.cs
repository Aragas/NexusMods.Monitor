using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.Mods
{
    public interface IModQueries
    {
        Task<ModViewModel?> GetAsync(uint gameId, uint modId, CancellationToken ct = default);
        Task<ModViewModel?> GetAsync(string gameDomain, uint modId, CancellationToken ct = default);
    }
}