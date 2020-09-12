using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using NexusMods.Monitor.Bot.Discord.Application.Options;
using NexusMods.Monitor.Bot.Discord.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application
{
    public sealed class SubscriptionRepository : ISubscriptionRepository, IUnitOfWork
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SubscriptionsOptions _subscriptionsOptions;

        public IUnitOfWork UnitOfWork => this;
        private bool _isSuccessful = true; // TODO:

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
                if (!subscriptionDTO.SubscriberId.StartsWith("Discord:"))
                    continue;

                yield return new SubscriptionEntity(ulong.Parse(subscriptionDTO.SubscriberId.Remove(0, 8)), subscriptionDTO.NexusModsGameId, subscriptionDTO.NexusModsModId);
            }
        }

        public async Task AddAsync(SubscriptionEntity subscriptionEntity)
        {
            var response = await _httpClientFactory.CreateClient().PutAsync($"{_subscriptionsOptions.APIEndpointV1}/subscribe",
                new StringContent(JsonConvert.SerializeObject(new SubscribeDTO($"Discord:{subscriptionEntity.ChannelId}", subscriptionEntity.NexusModsGameId, subscriptionEntity.NexusModsModId)), Encoding.UTF8, "application/json"));
            _isSuccessful = response.IsSuccessStatusCode;
        }

        public async Task RemoveAsync(SubscriptionEntity subscriptionEntity)
        {
            var response = await _httpClientFactory.CreateClient().PutAsync($"{_subscriptionsOptions.APIEndpointV1}/unsubscribe",
                new StringContent(JsonConvert.SerializeObject(new UnsubscribeDTO($"Discord:{subscriptionEntity.ChannelId}", subscriptionEntity.NexusModsGameId, subscriptionEntity.NexusModsModId)), Encoding.UTF8, "application/json"));
            _isSuccessful = response.IsSuccessStatusCode;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(_isSuccessful ? 1 : 0);
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default) => await SaveChangesAsync(cancellationToken) == 1;

        public void Dispose() { }

        private class SubscriptionDTO
        {
            [JsonProperty("subscriberId")]
            public string SubscriberId { get; private set; }= default!;
            [JsonProperty("nexusModsGameId")]
            public uint NexusModsGameId { get; private set;}= default!;
            [JsonProperty("nexusModsModId")]
            public uint NexusModsModId { get; private set;}= default!;

            private SubscriptionDTO() { }
        }
        private class SubscribeDTO
        {
            [JsonProperty("subscriberId")]
            public string SubscriberId { get; private set; } = default!;
            [JsonProperty("nexusModsGameId")]
            public uint NexusModsGameId { get; private set; } = default!;
            [JsonProperty("nexusModsModId")]
            public uint NexusModsModId { get; private set;} = default!;

            private SubscribeDTO() { }
            public SubscribeDTO(string subscriberId, uint nexusModsGameId, uint nexusModsModId)
            {
                SubscriberId = subscriberId;
                NexusModsGameId = nexusModsGameId;
                NexusModsModId = nexusModsModId;
            }
        }
        private class UnsubscribeDTO
        {
            [JsonProperty("subscriberId")]
            public string SubscriberId { get; private set; } = default!;
            [JsonProperty("nexusModsGameId")]
            public uint NexusModsGameId { get; private set;} = default!;
            [JsonProperty("nexusModsModId")]
            public uint NexusModsModId { get; private set;} = default!;

            private UnsubscribeDTO() { }
            public UnsubscribeDTO(string subscriberId, uint nexusModsGameId, uint nexusModsModId)
            {
                SubscriberId = subscriberId;
                NexusModsGameId = nexusModsGameId;
                NexusModsModId = nexusModsModId;
            }
        }
    }
}