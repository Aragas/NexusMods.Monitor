using NexusMods.Monitor.Shared.Common;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.Subscriptions
{
    public sealed class SubscriptionQueries : ISubscriptionQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public SubscriptionQueries(IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async IAsyncEnumerable<SubscriptionViewModel> GetAllAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClientFactory.CreateClient("Subscriptions.API").GetAsync("all", ct);
            }
            catch (Exception e) when (e is TaskCanceledException)
            {
                yield break;
            }

            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                foreach (var tuple in _jsonSerializer.Deserialize<SubscriptionDTO[]?>(content) ?? Array.Empty<SubscriptionDTO>())
                {
                    var (nexusModsGameId, nexusModsModId) = tuple;
                    yield return new SubscriptionViewModel(nexusModsGameId, nexusModsModId);
                }
            }

            response.Dispose();
        }

        private sealed record SubscriptionDTO(uint NexusModsGameId, uint NexusModsModId);
    }
}