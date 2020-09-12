using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Application.Queries.Subscriptions
{
    public sealed class SubscriptionViewModelComparer : IEqualityComparer<SubscriptionViewModel>
    {
        public bool Equals(SubscriptionViewModel? x, SubscriptionViewModel? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.NexusModsModId == y.NexusModsModId && x.NexusModsGameId == y.NexusModsGameId;
        }

        public int GetHashCode(SubscriptionViewModel obj)
        {
            return HashCode.Combine(obj.NexusModsModId, obj.NexusModsGameId);
        }
    }
}