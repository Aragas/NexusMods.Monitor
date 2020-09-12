using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Application.Queries.Subscriptions
{
    public interface ISubscriptionQueries
    {
        IAsyncEnumerable<SubscriptionViewModel> GetAllAsync();
    }
}