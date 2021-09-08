using NexusMods.Monitor.Shared.Application;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsGames
{
    public sealed class NexusModsGameQueries : INexusModsGameQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public NexusModsGameQueries(IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<NexusModsGameViewModel?> GetAsync(uint gameId, CancellationToken ct = default)
        {
            using var response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync($"game/id?gameId={gameId}", ct);
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                if (_jsonSerializer.Deserialize<GameDTO?>(content) is { } tuple)
                {
                    var (id, name, forumUrl, url, domainName) = tuple;
                    return new NexusModsGameViewModel(id, name, forumUrl, url, domainName);
                }
            }
            return null;
        }

        public async Task<NexusModsGameViewModel?> GetAsync(string gameDomain, CancellationToken ct = default)
        {
            using var response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync($"game/domain?gameDomain={gameDomain}", ct);
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                if (_jsonSerializer.Deserialize<GameDTO?>(content) is { } tuple)
                {
                    var (id, name, forumUrl, url, domainName) = tuple;
                    return new NexusModsGameViewModel(id, name, forumUrl, url, domainName);
                }
            }
            return null;
        }

        public async IAsyncEnumerable<NexusModsGameViewModel> GetAllAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            using var response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync("game/all", ct);
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                foreach (var tuple in _jsonSerializer.Deserialize<GameDTO[]?>(content) ?? Array.Empty<GameDTO>())
                {
                    var (id, name, forumUrl, url, domainName) = tuple;
                    yield return new NexusModsGameViewModel(id, name, forumUrl, url, domainName);
                }
            }
        }

        private sealed record GameDTO(uint Id, string Name, string ForumUrl, string Url, string DomainName);
    }
}