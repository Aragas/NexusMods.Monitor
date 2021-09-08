using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Metadata.Application.Extensions;

using NexusModsNET;
using NexusModsNET.Inquirers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.NexusModsGames
{
    public sealed class NexusModsGameQueries : INexusModsGameQueries
    {
        private readonly ILogger _logger;
        private readonly INexusModsClient _nexusModsClient;
        private readonly IMemoryCache _cache;

        public NexusModsGameQueries(ILogger<NexusModsGameQueries> logger, INexusModsClient nexusModsClient, IMemoryCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _nexusModsClient = nexusModsClient ?? throw new ArgumentNullException(nameof(nexusModsClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<NexusModsGameViewModel?> GetAsync(uint gameId, CancellationToken ct = default) => await GetAllAsync(ct).FirstOrDefaultAsync(x => x.Id == gameId, ct);
        public async Task<NexusModsGameViewModel?> GetAsync(string gameDomain, CancellationToken ct = default) => await GetAllAsync(ct).FirstOrDefaultAsync(x => x.DomainName == gameDomain, ct);

        public async IAsyncEnumerable<NexusModsGameViewModel> GetAllAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            if (!_cache.TryGetValue("games", out NexusModsGameViewModel[]? cacheEntry))
            {
                var gameInquirer = new GamesInquirer(_nexusModsClient);
                var games = await gameInquirer.GetGamesAsync(cancellationToken: ct);

                cacheEntry = games.Select(g => new NexusModsGameViewModel((uint) g.Id, g.Name, g.ForumUrl.ToString(), g.NexusmodsUrl.ToString(), g.DomainName)).ToArray();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromHours(8));
                _cache.Set("games", cacheEntry, cacheEntryOptions);
            }

            foreach (var nexusModsGame in cacheEntry)
                yield return nexusModsGame;
        }
    }
}