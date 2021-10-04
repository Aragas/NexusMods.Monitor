using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Metadata.Application.Extensions;
using NexusMods.Monitor.Metadata.Application.Queries.Games;
using NexusMods.Monitor.Shared.Common;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.Mods
{
    public sealed class ModQueries : IModQueries
    {
        [SuppressMessage("CodeQuality", "IDE0052", Justification = "Reserved for future use")]
        private readonly ILogger _logger;
        private readonly IGameQueries _nexusModsGameQueries;
        private readonly IDistributedCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public ModQueries(ILogger<ModQueries> logger, IGameQueries nexusModsGameQueries, IDistributedCache cache, IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _nexusModsGameQueries = nexusModsGameQueries ?? throw new ArgumentNullException(nameof(nexusModsGameQueries));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<ModViewModel?> GetAsync(uint gameId, uint modId, CancellationToken ct = default)
        {
            var games = _nexusModsGameQueries.GetAllAsync(ct);
            var gameDomain = (await games.FirstOrDefaultAsync(x => x.Id == gameId, ct))?.DomainName ?? "ERROR";

            return await GetAsync(gameDomain, modId, ct);
        }

        public async Task<ModViewModel?> GetAsync(string gameDomain, uint modId, CancellationToken ct = default)
        {
            var key = $"mod({gameDomain}, {modId})";
            if (!_cache.TryGetValue(key, _jsonSerializer, out ModViewModel? cacheEntry))
            {
                var response = await _httpClientFactory.CreateClient("NexusMods.API").GetAsync($"v1/games/{gameDomain}/mods/{modId}.json", ct);
                if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
                {
                    var content = await response.Content.ReadAsStringAsync(ct);
                    var mod = _jsonSerializer.Deserialize<ModDTO?>(content);
                    if (mod is not null)
                    {
                        cacheEntry = new ModViewModel((uint) mod.ModId, mod.Name);
                        var cacheEntryOptions = new DistributedCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromHours(8));
                        await _cache.SetAsync(key, cacheEntry, cacheEntryOptions, _jsonSerializer, ct);
                    }
                }
            }

            return cacheEntry;
        }

        private record ModDTO([property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("mod_id")] long ModId);
    }
}