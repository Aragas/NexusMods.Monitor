using System.Collections.Generic;

namespace NexusMods.Monitor.Bot.Slack.Application.Queries
{
    public interface ISubscriptionQueries
    {
        IAsyncEnumerable<SubscriptionViewModel> GetSubscriptionsAsync();
    }
}