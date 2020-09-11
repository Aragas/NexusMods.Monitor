using NexusMods.Monitor.Shared.Domain.SeedWork;

using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.SubscriptionAggregate
{
    public interface ISubscriptionRepository : IRepository<SubscriptionEntity>
    {
        IAsyncEnumerable<SubscriptionEntity> GetAllAsync();
    }
}