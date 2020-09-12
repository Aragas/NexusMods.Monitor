using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using NexusMods.Monitor.Scraper.Application.Options;

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NexusMods.Monitor.Scraper.Application.Queries.Subscriptions
{
    public sealed class SubscriptionQueries : ISubscriptionQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SubscriptionsOptions _options;

        public SubscriptionQueries(IHttpClientFactory httpClientFactory, IOptions<SubscriptionsOptions> options)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async IAsyncEnumerable<SubscriptionViewModel> GetAllAsync()
        {
            using var response = await _httpClientFactory.CreateClient().GetAsync($"{_options.APIEndpointV1}/all");
            var content = await response.Content.ReadAsStringAsync();
            var subscriptionDTOs = JsonConvert.DeserializeObject<SubscriptionDTO[]?>(content);
            foreach (var subscriptionDTO in subscriptionDTOs ?? Array.Empty<SubscriptionDTO>())
            {
                yield return new SubscriptionViewModel(subscriptionDTO.NexusModsGameId, subscriptionDTO.NexusModsModId);
            }
        }

        private class SubscriptionDTO
        {
            [JsonProperty("nexusModsGameId")]
            public uint NexusModsGameId { get; private set; } = default!;
            [JsonProperty("nexusModsModId")]
            public uint NexusModsModId { get; private set; } = default!;

            private SubscriptionDTO() { }
        }
    }
}