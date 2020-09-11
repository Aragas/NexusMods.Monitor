using NexusMods.Monitor.Scraper.Domain.AggregatesModel.SubscriptionAggregate;

using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Domain.Comparators
{
    public class SubscriptionEntityComparer : IEqualityComparer<SubscriptionEntity>
    {
        public bool Equals(SubscriptionEntity? x, SubscriptionEntity? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.NexusModsModId == y.NexusModsModId && x.NexusModsGameId == y.NexusModsGameId;
        }

        public int GetHashCode(SubscriptionEntity obj)
        {
            return HashCode.Combine(obj.NexusModsModId, obj.NexusModsGameId);
        }
    }
}