using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsGameAggregate;
using NexusMods.Monitor.Shared.Domain.SeedWork;
using NexusModsNET;
using NexusModsNET.Inquirers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Infrastructure.Repositories
{
    public class NexusModsGameRepository : INexusModsGameRepository
    {
        private readonly ILogger _logger;
        private readonly INexusModsClient _nexusModsClient;
        private readonly IMemoryCache _memoryCache;

        public NexusModsGameRepository(ILogger<NexusModsGameRepository> logger, INexusModsClient nexusModsClient, IMemoryCache memoryCache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _nexusModsClient = nexusModsClient ?? throw new ArgumentNullException(nameof(nexusModsClient));
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
                var gameInquirer = new GamesInquirer(_nexusModsClient);
                var games = await gameInquirer.GetGamesAsync();

                cacheEntry = games.Select(g => new NexusModsGameEntity((uint) g.Id, g.Name, g.ForumUrl.ToString(), g.NexusmodsUrl.ToString(), g.DomainName)).ToArray(); 
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromHours(8));
                _memoryCache.Set("games", cacheEntry, cacheEntryOptions);
            }

            foreach (var nexusModsGame in cacheEntry)
                yield return nexusModsGame;
        }
    }
}