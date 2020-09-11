﻿using NexusMods.Monitor.Shared.Domain.SeedWork;

namespace NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate
{
    public interface ISubscriptionRepository : IRepository<SubscriptionEntity>
    {
        SubscriptionEntity Add(SubscriptionEntity subscriptionEntity);
        SubscriptionEntity Remove(SubscriptionEntity subscriptionEntity);
    }
}