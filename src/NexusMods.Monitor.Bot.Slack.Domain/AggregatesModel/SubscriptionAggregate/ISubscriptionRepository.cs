using NexusMods.Monitor.Shared.Domain.SeedWork;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Domain.AggregatesModel.SubscriptionAggregate
{
    public interface ISubscriptionRepository : IRepository<SubscriptionEntity>
    {
        IAsyncEnumerable<SubscriptionEntity> GetAllAsync();

        Task SubscribeAsync(string channelId, uint gameId, uint modId);
        Task UnsubscribeAsync(string channelId, uint gameId, uint modId);
    }
}