using NexusMods.Monitor.Shared.Domain.SeedWork;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsGameAggregate
{
    public interface  INexusModsGameRepository : IRepository<NexusModsGameEntity>
    {
        Task<NexusModsGameEntity?> GetAsync(uint gameId);
        IAsyncEnumerable<NexusModsGameEntity> GetAllAsync();
    }
}