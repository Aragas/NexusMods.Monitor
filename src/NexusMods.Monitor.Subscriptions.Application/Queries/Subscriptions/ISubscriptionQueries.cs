using System.Collections.Generic;
using System.Threading;

namespace NexusMods.Monitor.Subscriptions.Application.Queries.Subscriptions
{
    public interface ISubscriptionQueries
    {
        IAsyncEnumerable<SubscriptionViewModel> GetSubscriptionsAsync(CancellationToken ct = default);
    }
}