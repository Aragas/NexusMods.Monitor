using Microsoft.EntityFrameworkCore;

using NexusMods.Monitor.Subscriptions.Infrastructure.Contexts;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusMods.Monitor.Subscriptions.Application.Queries
{
    public class SubscriptionQueries : ISubscriptionQueries
    {
        private readonly SubscriptionDb _context;

        public SubscriptionQueries(SubscriptionDb context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IAsyncEnumerable<SubscriptionViewModel> GetSubscriptionsAsync() =>
            _context.SubscriptionEntities.AsNoTracking().AsAsyncEnumerable().Select(x => new SubscriptionViewModel(x.SubscriberId, x.NexusModsGameId, x.NexusModsModId));
    }
}