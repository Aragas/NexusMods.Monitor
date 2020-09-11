using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using NexusMods.Monitor.Scraper.Application.Options;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace NexusMods.Monitor.Scraper.Application
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SubscriptionsOptions _subscriptionsOptions;

        public IUnitOfWork UnitOfWork => ReadOnlyUnitOfWork.Instance;

        public SubscriptionRepository(IHttpClientFactory httpClientFactory, IOptions<SubscriptionsOptions> options)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _subscriptionsOptions = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async IAsyncEnumerable<SubscriptionEntity> GetAllAsync()
        {
            using var response = await _httpClientFactory.CreateClient().GetAsync($"{_subscriptionsOptions.APIEndpointV1}/all");
            var content = await response.Content.ReadAsStringAsync();
            var subscriptionDTOs = JsonConvert.DeserializeObject<SubscriptionDTO[]?>(content);
            foreach (var subscriptionDTO in subscriptionDTOs ?? Array.Empty<SubscriptionDTO>())
            {
                yield return new SubscriptionEntity(subscriptionDTO.NexusModsGameId, subscriptionDTO.NexusModsModId);
            }
        }

        private class SubscriptionDTO
        {
            [JsonProperty("nexusModsGameId")]
            public uint NexusModsGameId { get; private set; }
            [JsonProperty("nexusModsModId")]
            public uint NexusModsModId { get; private set; }

            private SubscriptionDTO() { }
        }
    }
}