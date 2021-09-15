using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Common.Extensions
{
    public static class ImmutableArrayExtensions
    {
        public static async Task<ImmutableArray<TSource>> ToImmutableArrayAsync<TSource>(this IAsyncEnumerable<TSource> items, CancellationToken ct = default)
        {
            return ImmutableArray.Create(await items.ToArrayAsync(ct));
        }
    }
}