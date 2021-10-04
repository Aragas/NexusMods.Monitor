
using Microsoft.Extensions.Caching.Distributed;

namespace NexusMods.Monitor.Metadata.Application.Extensions
{
    public static class DistributedCacheEntryOptionsExtensions
    {
        public static DistributedCacheEntryOptions SetSize(this DistributedCacheEntryOptions options, int size) => options;
    }
}