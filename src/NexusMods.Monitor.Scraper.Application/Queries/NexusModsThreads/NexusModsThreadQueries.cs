using NexusMods.Monitor.Shared.Common;

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsThreads
{
    public sealed class NexusModsThreadQueries : INexusModsThreadQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public NexusModsThreadQueries(IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<NexusModsThreadViewModel?> GetAsync(uint gameIdRequest, uint modIdRequest, CancellationToken ct = default)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync(
                    $"thread/id?gameId={gameIdRequest}&modId={modIdRequest}",
                    HttpCompletionOption.ResponseHeadersRead,
                    ct);
            }
            catch (Exception e) when (e is TaskCanceledException)
            {
                return null;
            }

            try
            {
                if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
                {
                    var content = await response.Content.ReadAsStreamAsync(ct);
                    if (await _jsonSerializer.DeserializeAsync<ThreadDTO?>(content, ct) is { } tuple)
                    {
                        var (gameId, modId, threadId) = tuple;
                        return new NexusModsThreadViewModel(gameId, modId, threadId);
                    }
                }
                return null;
            }
            finally
            {
                response.Dispose();
            }
        }

        private sealed record ThreadDTO(uint GameId, uint ModId, uint ThreadId);

    }
}