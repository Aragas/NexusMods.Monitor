using Microsoft.EntityFrameworkCore;

using NexusMods.Monitor.Subscriptions.Infrastructure.Contexts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NexusMods.Monitor.Subscriptions.Application.Queries.Subscriptions
{
    public sealed class SubscriptionQueries : ISubscriptionQueries
    {
        private readonly SubscriptionDb _context;

        public SubscriptionQueries(SubscriptionDb context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IAsyncEnumerable<SubscriptionViewModel> GetSubscriptionsAsync(CancellationToken ct = default) => _context.SubscriptionEntities
            .AsNoTracking()
            .Select(x => new SubscriptionViewModel(x.SubscriberId, x.NexusModsGameId, x.NexusModsModId))
            .ToAsyncEnumerable();
    }
}