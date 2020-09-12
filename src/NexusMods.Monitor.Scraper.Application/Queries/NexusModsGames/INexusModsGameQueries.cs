using System.Collections.Generic;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsGames
{
    public interface INexusModsGameQueries
    {
        Task<NexusModsGameViewModel?> GetAsync(uint gameId);
        IAsyncEnumerable<NexusModsGameViewModel> GetAllAsync();
    }
}