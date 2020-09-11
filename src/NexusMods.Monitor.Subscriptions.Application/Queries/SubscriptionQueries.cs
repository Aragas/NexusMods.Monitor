using Microsoft.EntityFrameworkCore;

using NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Subscriptions.Infrastructure.Contexts;

using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Subscriptions.Application.Queries
{
    public class SubscriptionQueries : ISubscriptionQueries
    {
        private readonly SubscriptionDb _context;

        public SubscriptionQueries(SubscriptionDb context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IAsyncEnumerable<SubscriptionEntity> GetSubscriptionsAsync() => _context.SubscriptionEntities.AsNoTracking().AsAsyncEnumerable();
    }
}