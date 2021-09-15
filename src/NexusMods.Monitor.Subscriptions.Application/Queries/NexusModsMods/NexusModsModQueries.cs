using NexusMods.Monitor.Shared.Application;
using NexusMods.Monitor.Shared.Common;

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.Application.Queries.NexusModsMods
{
    public sealed class NexusModsModQueries : INexusModsModQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public NexusModsModQueries(IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<NexusModsModViewModel?> GetAsync(uint gameId, uint modId, CancellationToken ct = default)
        {
            using var response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync($"mod/id?gameId={gameId}&modId={modId}", ct);
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                if (_jsonSerializer.Deserialize<ModDTO?>(content) is { } tuple)
                {
                    var (id, name) = tuple;
                    return new NexusModsModViewModel(id, name);
                }
            }
            return null;
        }

        private sealed record ModDTO(uint Id, string Name);
    }
}