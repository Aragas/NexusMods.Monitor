using NexusMods.Monitor.Shared.Domain.SeedWork;

using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate
{
    public interface ISubscriptionRepository : IRepository<SubscriptionEntity>
    {
        SubscriptionEntity Add(SubscriptionEntity subscriptionEntity);
        SubscriptionEntity Remove(SubscriptionEntity subscriptionEntity);
        Task<SubscriptionEntity?> GetAsync(string subscriberId, uint nexusModsGameId, uint nexusModsModId);
    }
}