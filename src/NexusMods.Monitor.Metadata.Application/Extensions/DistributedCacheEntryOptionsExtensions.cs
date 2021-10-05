
using Microsoft.Extensions.Caching.Distributed;

using System.Diagnostics.CodeAnalysis;

namespace NexusMods.Monitor.Metadata.Application.Extensions
{
    public static class DistributedCacheEntryOptionsExtensions
    {
        [SuppressMessage("Style", "IDE0060", Justification = "Public API")]
        public static DistributedCacheEntryOptions SetSize(this DistributedCacheEntryOptions options, int size) => options;
    }
}