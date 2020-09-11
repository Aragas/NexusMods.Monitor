using AngleSharp;

using Microsoft.EntityFrameworkCore;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsGameAggregate;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsThreadAggregate;
using NexusMods.Monitor.Scraper.Infrastructure.Contexts;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Infrastructure.Repositories
{
    public class NexusModsThreadRepository : INexusModsThreadRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly INexusModsGameRepository _nexusModsGameRepository;
        private readonly NexusModsDb _nexusModsDb;

        public IUnitOfWork UnitOfWork => _nexusModsDb;

        public NexusModsThreadRepository(IHttpClientFactory httpClientFactory, INexusModsGameRepository nexusModsGameRepository, NexusModsDb nexusModsDb)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _nexusModsGameRepository = nexusModsGameRepository ?? throw new ArgumentNullException(nameof(nexusModsGameRepository));
            _nexusModsDb = nexusModsDb ?? throw new ArgumentNullException(nameof(nexusModsDb));
        }

        public async Task<NexusModsThreadEntity> GetAsync(uint gameId, uint modId)
        {
            if (await _nexusModsDb.ThreadIdEntities.FindAsync(gameId, modId) is { } threadIdEntity)
                return threadIdEntity;

            var games = _nexusModsGameRepository.GetAllAsync();
            var gameIdText = (await games.FirstOrDefaultAsync(x => x.Id == gameId))?.DomainName ?? "ERROR";

            using var response = await _httpClientFactory.CreateClient().GetAsync($"https://www.nexusmods.com/{gameIdText}/mods/{modId}");
            var content = await response.Content.ReadAsStringAsync();

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(request => request.Content(content));

            var element = document.GetElementById("mod-page-tab-posts");
            var dataTarget = element.Children[0]?.GetAttribute("data-target");
            var split = dataTarget?.Split("thread_id=", StringSplitOptions.RemoveEmptyEntries);
            if (split?.Length > 1)
            {
                var split2 = split[1].Split('&', StringSplitOptions.RemoveEmptyEntries);
                if (split2.Length > 0 && uint.TryParse(split2[0], out var threadId))
                {
                    var entity = new NexusModsThreadEntity(threadId, gameId, modId);
                    _nexusModsDb.Entry(entity).State = EntityState.Added;
                    await _nexusModsDb.SaveChangesAsync();
                    return entity;
                }
            }

            throw new Exception();
        }
    }
}