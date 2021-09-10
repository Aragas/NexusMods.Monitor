using System.Collections.Generic;
using System.Threading;

namespace NexusMods.Monitor.Bot.Discord.Application.Queries.Subscriptions
{
    public interface ISubscriptionQueries
    {
        IAsyncEnumerable<SubscriptionViewModel> GetAllAsync(CancellationToken ct = default);
    }
}