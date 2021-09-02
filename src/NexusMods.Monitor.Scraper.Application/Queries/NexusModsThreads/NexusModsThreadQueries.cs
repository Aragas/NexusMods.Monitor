using AngleSharp;

using Microsoft.Extensions.Caching.Memory;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsGames;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsThreads
{
    public sealed class NexusModsThreadQueries : INexusModsThreadQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly INexusModsGameQueries _nexusModsGameQueries;
        private readonly IMemoryCache _memoryCache;

        public NexusModsThreadQueries(IHttpClientFactory httpClientFactory, INexusModsGameQueries nexusModsGameQueries, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _nexusModsGameQueries = nexusModsGameQueries ?? throw new ArgumentNullException(nameof(nexusModsGameQueries));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<NexusModsThreadViewModel> GetAsync(uint gameId, uint modId)
        {
            var key = $"thread_id({gameId}, {modId})";
            if (!_memoryCache.TryGetValue(key, out NexusModsThreadViewModel cacheEntry))
            {
                var games = _nexusModsGameQueries.GetAllAsync();
                var gameIdText = (await games.FirstOrDefaultAsync(x => x.Id == gameId))?.DomainName ?? "ERROR";

                using var response = await _httpClientFactory.CreateClient().GetAsync($"https://www.nexusmods.com/{gameIdText}/mods/{modId}");
                var content = await response.Content.ReadAsStringAsync();

                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(request => request.Content(content));

                var element = document.GetElementById("mod-page-tab-posts");
                var dataTarget = element?.Children[0]?.GetAttribute("data-target");
                var split = dataTarget?.Split("thread_id=", StringSplitOptions.RemoveEmptyEntries);
                if (split?.Length > 1)
                {
                    var split2 = split[1].Split('&', StringSplitOptions.RemoveEmptyEntries);
                    if (split2.Length > 0 && uint.TryParse(split2[0], out var threadId))
                        cacheEntry = new NexusModsThreadViewModel(gameId, modId, threadId);
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromHours(8));
                _memoryCache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}