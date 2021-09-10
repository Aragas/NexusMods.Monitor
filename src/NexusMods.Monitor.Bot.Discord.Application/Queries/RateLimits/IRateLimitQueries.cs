using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.Queries.RateLimits
{
    public interface IRateLimitQueries
    {
        Task<RateLimitViewModel?> GetAsync(CancellationToken ct = default);
    }
}