using Microsoft.Extensions.Caching.Distributed;

using NexusMods.Monitor.Shared.Common;

using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Extensions
{

    public static class DistributedCacheExtensions
    {
        public static bool TryGetValue<TItem>(this IDistributedCache cache, string key, DefaultJsonSerializer jsonSerializer, out TItem? value)
        {
            if (cache.Get(key) is { } data)
            {
                value = jsonSerializer.Deserialize<TItem?>(data);
                return true;
            }

            value = default;
            return false;
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options, DefaultJsonSerializer jsonSerializer, CancellationToken ct)
        {
            var valueData = jsonSerializer.SerializeToUtf8Bytes(value);
            await cache.SetAsync(key, valueData, options, ct);
        }
    }
}