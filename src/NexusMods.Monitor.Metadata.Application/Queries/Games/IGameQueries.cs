using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.Games
{
    public interface IGameQueries
    {
        Task<GameViewModel?> GetAsync(uint gameId, CancellationToken ct = default);
        Task<GameViewModel?> GetAsync(string gameDomain, CancellationToken ct = default);
        IAsyncEnumerable<GameViewModel> GetAllAsync(CancellationToken ct = default);
    }
}