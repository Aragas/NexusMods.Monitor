using System.Collections.Generic;
using System.Threading;

namespace NexusMods.Monitor.Scraper.Application.Queries.Subscriptions
{
    public interface ISubscriptionQueries
    {
        IAsyncEnumerable<SubscriptionViewModel> GetAllAsync(CancellationToken ct = default);
    }
}