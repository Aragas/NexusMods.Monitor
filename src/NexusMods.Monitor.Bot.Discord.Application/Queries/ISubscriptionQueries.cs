using System.Collections.Generic;
using System.Threading;

namespace NexusMods.Monitor.Bot.Discord.Application.Queries
{
    public interface ISubscriptionQueries
    {
        IAsyncEnumerable<SubscriptionViewModel> GetAllAsync(CancellationToken ct = default);
    }
}