using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using NexusMods.Monitor.Scraper.Application.Options;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsGameAggregate;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application
{
    public class NexusModsGameRepository : INexusModsGameRepository
    {
        private readonly ILogger _logger;
        private readonly NexusModsOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;

        public NexusModsGameRepository(ILogger<NexusModsGameRepository> logger, IOptions<NexusModsOptions> options, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public IUnitOfWork UnitOfWork => ReadOnlyUnitOfWork.Instance;

        public async Task<NexusModsGameEntity?> GetAsync(uint gameId)
        {
            var games = GetAllAsync();
            return await games.FirstOrDefaultAsync(x => x.Id == gameId);
        }
        public async IAsyncEnumerable<NexusModsGameEntity> GetAllAsync()
        {
            if (!_memoryCache.TryGetValue("games", out NexusModsGameEntity[] cacheEntry))
            {
                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.nexusmods.com/v1/games.json");
                requestMessage.Headers.Add("apikey", _options.APIKey);
                using var response = await _httpClientFactory.CreateClient().SendAsync(requestMessage);
                var content = await response.Content.ReadAsStringAsync();
                cacheEntry = JsonConvert.DeserializeObject<NexusModsGameDTO[]>(content).Select(x => new NexusModsGameEntity(x.Id, x.Name, x.ForumUrl, x.NexusModsUrl, x.DomainName)).ToArray();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetAbsoluteExpiration(TimeSpan.FromHours(8));

                _memoryCache.Set("games", cacheEntry, cacheEntryOptions);
            }

            foreach (var nexusModsGame in cacheEntry)
                yield return nexusModsGame;
        }

        private sealed class NexusModsGameDTO
        {
            [JsonProperty("id")]
            public uint Id { get; private set; } = default!;
            [JsonProperty("name")]
            public string Name { get; private set; } = default!;
            [JsonProperty("forum_url")]
            public string ForumUrl { get; private set; } = default!;
            [JsonProperty("nexusmods_url")]
            public string NexusModsUrl { get; private set; } = default!;
            [JsonProperty("domain_name")]
            public string DomainName { get; private set; } = default!;

            private NexusModsGameDTO() { }
        }
    }
}