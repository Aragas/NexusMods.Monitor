using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Subscriptions.Domain.ValueObject
{
    public record SubscriptionId(string SubscriberId, uint NexusModsGameId, uint NexusModsModId) : IComparable<SubscriptionId>, IComparable
    {
        public static bool operator <(SubscriptionId left, SubscriptionId right) => Comparer<SubscriptionId>.Default.Compare(left, right) < 0;
        public static bool operator >(SubscriptionId left, SubscriptionId right) => Comparer<SubscriptionId>.Default.Compare(left, right) > 0;
        public static bool operator <=(SubscriptionId left, SubscriptionId right) => Comparer<SubscriptionId>.Default.Compare(left, right) <= 0;
        public static bool operator >=(SubscriptionId left, SubscriptionId right) => Comparer<SubscriptionId>.Default.Compare(left, right) >= 0;

        public string SubscriberId { get; set; } = SubscriberId;
        public uint NexusModsGameId { get; set; } = NexusModsGameId;
        public uint NexusModsModId { get; set; } = NexusModsModId;

        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is SubscriptionId other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(SubscriptionId)}");
        }

        public int CompareTo(SubscriptionId other)
        {
            var subscriberIdComparison = string.Compare(SubscriberId, other.SubscriberId, StringComparison.Ordinal);
            if (subscriberIdComparison != 0) return subscriberIdComparison;
            var nexusModsGameIdComparison = NexusModsGameId.CompareTo(other.NexusModsGameId);
            if (nexusModsGameIdComparison != 0) return nexusModsGameIdComparison;
            return NexusModsModId.CompareTo(other.NexusModsModId);
        }
    }
}