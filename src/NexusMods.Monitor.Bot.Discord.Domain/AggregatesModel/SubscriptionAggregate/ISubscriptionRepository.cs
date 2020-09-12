using NexusMods.Monitor.Shared.Domain.SeedWork;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Domain.AggregatesModel.SubscriptionAggregate
{
    public interface ISubscriptionRepository : IRepository<SubscriptionEntity>
    {
        IAsyncEnumerable<SubscriptionEntity> GetAllAsync();

        Task AddAsync(SubscriptionEntity subscriptionEntity);
        Task RemoveAsync(SubscriptionEntity subscriptionEntity);
    }
}