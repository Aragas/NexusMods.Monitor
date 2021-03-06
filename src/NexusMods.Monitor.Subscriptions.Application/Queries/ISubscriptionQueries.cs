﻿using System.Collections.Generic;

namespace NexusMods.Monitor.Subscriptions.Application.Queries
{
    public interface ISubscriptionQueries
    {
        IAsyncEnumerable<SubscriptionViewModel> GetSubscriptionsAsync();
    }
}