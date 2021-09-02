using Microsoft.Extensions.Options;

using NexusMods.Monitor.Scraper.Application.Options;
using NexusMods.Monitor.Shared.Application;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;

namespace NexusMods.Monitor.Scraper.Application.Queries.Subscriptions
{
    public sealed class SubscriptionQueries : ISubscriptionQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SubscriptionsOptions _options;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public SubscriptionQueries(IHttpClientFactory httpClientFactory, IOptions<SubscriptionsOptions> options, DefaultJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async IAsyncEnumerable<SubscriptionViewModel> GetAllAsync()
        {
            using var response = await _httpClientFactory.CreateClient("RetryClient").GetAsync($"{_options.APIEndpointV1}/all");
            var content = await response.Content.ReadAsStringAsync();
            var subscriptions = _jsonSerializer.Deserialize<SubscriptionDTO[]?>(content) ?? Array.Empty<SubscriptionDTO>();
            foreach (var (nexusModsGameId, nexusModsModId) in subscriptions)
            {
                yield return new SubscriptionViewModel(nexusModsGameId, nexusModsModId);
            }
        }

        private sealed record SubscriptionDTO(uint NexusModsGameId, uint NexusModsModId);
    }
}