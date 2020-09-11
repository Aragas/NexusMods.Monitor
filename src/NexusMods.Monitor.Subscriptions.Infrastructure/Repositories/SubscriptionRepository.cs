using Microsoft.EntityFrameworkCore;

using NexusMods.Monitor.Shared.Domain.SeedWork;
using NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Subscriptions.Infrastructure.Contexts;

using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Subscriptions.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly SubscriptionDb _context;

        public IUnitOfWork UnitOfWork => _context;

        public SubscriptionRepository(SubscriptionDb context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public SubscriptionEntity Add(SubscriptionEntity modToMonitorEntity)
        {
            _context.Entry(modToMonitorEntity).State = EntityState.Added;
            return modToMonitorEntity;
        }

        public IAsyncEnumerable<SubscriptionEntity> GetAllAsync() => _context.SubscriptionEntities.AsQueryable().AsAsyncEnumerable();

        public SubscriptionEntity Remove(SubscriptionEntity modToMonitorEntity)
        {
            _context.Entry(modToMonitorEntity).State = EntityState.Deleted;
            return modToMonitorEntity;
        }
    }
}