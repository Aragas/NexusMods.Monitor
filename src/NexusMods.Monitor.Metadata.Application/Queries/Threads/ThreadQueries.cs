using AngleSharp;

using Microsoft.Extensions.Caching.Memory;

using NexusMods.Monitor.Metadata.Application.Queries.Games;

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
        private readonly IMemoryCache _cache;

        public ThreadQueries(IHttpClientFactory httpClientFactory, IGameQueries nexusModsGameQueries, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _nexusModsGameQueries = nexusModsGameQueries ?? throw new ArgumentNullException(nameof(nexusModsGameQueries));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<ThreadViewModel> GetAsync(uint gameId, uint modId, CancellationToken ct = default)
        {
            var key = $"thread_id({gameId}, {modId})";
            if (!_cache.TryGetValue(key, out ThreadViewModel cacheEntry))
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

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromHours(8));
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}