using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.Queries.RateLimits
{
    public interface IRateLimitQueries
    {
        Task<RateLimitViewModel?> GetAsync(CancellationToken ct = default);
    }
}