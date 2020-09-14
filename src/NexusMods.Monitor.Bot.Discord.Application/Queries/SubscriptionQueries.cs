using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using NexusMods.Monitor.Bot.Discord.Application.Options;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;

namespace NexusMods.Monitor.Bot.Discord.Application.Queries
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
                if (!subscriptionDTO.SubscriberId.StartsWith("Discord:"))
                    continue;

                yield return new SubscriptionViewModel(ulong.Parse(subscriptionDTO.SubscriberId.Remove(0, 8)), subscriptionDTO.NexusModsGameId, subscriptionDTO.NexusModsModId);
            }
        }

        [DataContract]
        private sealed class SubscriptionDTO
        {
            [DataMember(Name = "subscriberId")]
            public string SubscriberId { get; private set; } = default!;
            [DataMember(Name = "nexusModsGameId")]
            public uint NexusModsGameId { get; private set;} = default!;
            [DataMember(Name = "nexusModsModId")]
            public uint NexusModsModId { get; private set;} = default!;

            private SubscriptionDTO() { }
        }
    }
}