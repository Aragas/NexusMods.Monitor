using AngleSharp;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

using NexusMods.Monitor.Metadata.Application.Extensions;
using NexusMods.Monitor.Metadata.Application.Queries.Games;
using NexusMods.Monitor.Shared.Common;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.Threads
{
    public sealed class ThreadQueries : IThreadQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IGameQueries _nexusModsGameQueries;
        private readonly IDistributedCache _cache;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public ThreadQueries(IHttpClientFactory httpClientFactory, IGameQueries nexusModsGameQueries, IDistributedCache cache, DefaultJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _nexusModsGameQueries = nexusModsGameQueries ?? throw new ArgumentNullException(nameof(nexusModsGameQueries));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<ThreadViewModel?> GetAsync(uint gameId, uint modId, CancellationToken ct = default)
        {
            var key = $"thread_id({gameId}, {modId})";
            if (!_cache.TryGetValue(key, _jsonSerializer, out ThreadViewModel? cacheEntry))
            {
                var games = _nexusModsGameQueries.GetAllAsync(ct);
                var gameIdText = (await games.FirstOrDefaultAsync(x => x.Id == gameId, ct))?.DomainName ?? "ERROR";

                using var response = await _httpClientFactory.CreateClient("NexusMods").GetAsync($"{gameIdText}/mods/{modId}", ct);
                var content = await response.Content.ReadAsStringAsync(ct);

                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(request => request.Content(content), ct);

                var element = document.GetElementById("mod-page-tab-posts");
                var dataTarget = element?.Children[0]?.GetAttribute("data-target");
                var split = dataTarget?.Split("thread_id=", StringSplitOptions.RemoveEmptyEntries);
                if (split?.Length > 1)
                {
                    var split2 = split[1].Split('&', StringSplitOptions.RemoveEmptyEntries);
                    if (split2.Length > 0 && uint.TryParse(split2[0], out var threadId))
                        cacheEntry = new ThreadViewModel(gameId, modId, threadId);
                }

                var cacheEntryOptions = new DistributedCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromHours(8));
                await _cache.SetAsync(key, cacheEntry, cacheEntryOptions, _jsonSerializer, ct);
            }

            return cacheEntry;
        }
    }
}