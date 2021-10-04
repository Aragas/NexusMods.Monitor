using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Metadata.Application.Extensions;
using NexusMods.Monitor.Shared.Common;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.Games
{
    public sealed class GameQueries : IGameQueries
    {
        [SuppressMessage("CodeQuality", "IDE0052", Justification = "Reserved for future use")]
        private readonly ILogger _logger;
        private readonly IDistributedCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public GameQueries(ILogger<GameQueries> logger, IDistributedCache cache, IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<GameViewModel?> GetAsync(uint gameId, CancellationToken ct = default) => await GetAllAsync(ct).FirstOrDefaultAsync(x => x.Id == gameId, ct);
        public async Task<GameViewModel?> GetAsync(string gameDomain, CancellationToken ct = default) => await GetAllAsync(ct).FirstOrDefaultAsync(x => x.DomainName == gameDomain, ct);

        public async IAsyncEnumerable<GameViewModel> GetAllAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            if (!_cache.TryGetValue("games", _jsonSerializer, out GameViewModel[]? cacheEntry))
            {
                var response = await _httpClientFactory.CreateClient("NexusMods.API").GetAsync(
                    "v1/games.json?include_unapproved=false",
                    HttpCompletionOption.ResponseHeadersRead,
                    ct);

                if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
                {
                    var content = await response.Content.ReadAsStreamAsync(ct);
                    var games = await _jsonSerializer.DeserializeAsync<GameDTO[]?>(content, ct) ?? Array.Empty<GameDTO>();

                    cacheEntry = games.Select(g => new GameViewModel(g.Id, g.Name, g.ForumUrl.ToString(), g.Url.ToString(), g.DomainName)).ToArray();
                    var cacheEntryOptions = new DistributedCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromHours(8));
                    await _cache.SetAsync("games", cacheEntry, cacheEntryOptions, _jsonSerializer, ct);
                }
            }

            foreach (var nexusModsGame in cacheEntry ?? Array.Empty<GameViewModel>())
                yield return nexusModsGame;
        }

        private record GameDTO([property: JsonPropertyName("id")] uint Id, [property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("forum_url")] string ForumUrl, [property: JsonPropertyName("nexusmods_url")] string Url, [property: JsonPropertyName("domain_name")] string DomainName);
    }
}