using System.Collections.Generic;

namespace NexusMods.Monitor.Bot.Discord.Application.Queries
{
    public interface ISubscriptionQueries
    {
        IAsyncEnumerable<SubscriptionViewModel> GetAllAsync();
    }
}