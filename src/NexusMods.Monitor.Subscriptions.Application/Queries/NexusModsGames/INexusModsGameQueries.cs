using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.Application.Queries.NexusModsGames
{
    public interface INexusModsGameQueries
    {
        Task<NexusModsGameViewModel?> GetAsync(string gameDomain, CancellationToken ct = default);
    }
}