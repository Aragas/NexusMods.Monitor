using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Metadata.Application.Queries.NexusModsGames;

using NexusModsNET;
using NexusModsNET.Inquirers;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.NexusModsMods
{
    public sealed class NexusModsModQueries : INexusModsModQueries
    {
        private readonly ILogger _logger;
        private readonly INexusModsClient _nexusModsClient;
        private readonly INexusModsGameQueries _nexusModsGameQueries;
        private readonly IMemoryCache _cache;

        public NexusModsModQueries(ILogger<NexusModsModQueries> logger, INexusModsClient nexusModsClient, INexusModsGameQueries nexusModsGameQueries, IMemoryCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _nexusModsClient = nexusModsClient ?? throw new ArgumentNullException(nameof(nexusModsClient));
            _nexusModsGameQueries =
                nexusModsGameQueries ?? throw new ArgumentNullException(nameof(nexusModsGameQueries));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<NexusModsModViewModel?> GetAsync(uint gameId, uint modId, CancellationToken ct = default)
        {
            var games = _nexusModsGameQueries.GetAllAsync(ct);
            var gameDomain = (await games.FirstOrDefaultAsync(x => x.Id == gameId, ct))?.DomainName ?? "ERROR";

            return await GetAsync(gameDomain, modId, ct);
        }

        public async Task<NexusModsModViewModel?> GetAsync(string gameDomain, uint modId,
            CancellationToken ct = default)
        {
            var key = $"mod({gameDomain}, {modId})";
            if (!_cache.TryGetValue(key, out NexusModsModViewModel cacheEntry))
            {
                var modInquirer = new ModsInquirer(_nexusModsClient);
                var mod = await modInquirer.GetMod(gameDomain, modId, ct);

                cacheEntry = new NexusModsModViewModel((uint) mod.ModId, mod.Name);

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1)
                    .SetAbsoluteExpiration(TimeSpan.FromHours(8));
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}