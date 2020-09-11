using NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate;

using System.Collections.Generic;

namespace NexusMods.Monitor.Subscriptions.Application.Queries
{
    public interface ISubscriptionQueries
    {
        IAsyncEnumerable<SubscriptionEntity> GetSubscriptionsAsync();
    }
}