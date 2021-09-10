using System.Collections.Generic;
using System.Threading;

namespace NexusMods.Monitor.Bot.Slack.Application.Queries.Subscriptions
{
    public interface ISubscriptionQueries
    {
        IAsyncEnumerable<SubscriptionViewModel> GetAllAsync(CancellationToken ct = default);
    }
}