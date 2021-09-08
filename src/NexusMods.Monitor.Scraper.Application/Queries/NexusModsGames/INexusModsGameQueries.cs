using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsGames
{
    public interface INexusModsGameQueries
    {
        Task<NexusModsGameViewModel?> GetAsync(uint gameId, CancellationToken ct = default);
        Task<NexusModsGameViewModel?> GetAsync(string gameDomain, CancellationToken ct = default);
        IAsyncEnumerable<NexusModsGameViewModel> GetAllAsync(CancellationToken ct = default);
    }
}