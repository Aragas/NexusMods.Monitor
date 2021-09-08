using Microsoft.Extensions.Caching.Distributed;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Extensions
{
    internal static class DistributedCacheExtensions
    {
        public static bool TryGetValueT<T>(this IDistributedCache cache, string key, [NotNullWhen(true)] out T? value)
        {
            var array = cache.Get(key);
            if (array is null)
            {
                value = default;
                return false;
            }

            value = JsonSerializer.Deserialize<T>(array);
            if (value is null)
                throw new NullReferenceException();
            return true;
        }

        public static async Task SetValueTAsync<T>(this IDistributedCache cache, string key, T? value, DistributedCacheEntryOptions options, CancellationToken ct = default)
        {
            var array = JsonSerializer.SerializeToUtf8Bytes(value);
            await cache.SetAsync(key, array, options, ct);
        }
    }
}