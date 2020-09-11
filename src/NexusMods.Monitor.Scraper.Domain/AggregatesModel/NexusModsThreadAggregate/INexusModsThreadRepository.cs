using NexusMods.Monitor.Shared.Domain.SeedWork;

using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsThreadAggregate
{
    public interface INexusModsThreadRepository : IRepository<NexusModsThreadEntity>
    {
        Task<NexusModsThreadEntity> GetAsync(uint gameId, uint modId);
    }
}