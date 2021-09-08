using Microsoft.Extensions.Options;

using NexusMods.Monitor.Bot.Discord.Application.Options;
using NexusMods.Monitor.Shared.Application;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NexusMods.Monitor.Bot.Discord.Application.Queries
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

        public async IAsyncEnumerable<SubscriptionViewModel> GetAllAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            using var response = await _httpClientFactory.CreateClient().GetAsync($"{_options.APIEndpointV1}/all", ct);
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                foreach (var (subscriberId, nexusModsGameId, nexusModsModId) in _jsonSerializer.Deserialize<SubscriptionDTO[]?>(content) ?? Array.Empty<SubscriptionDTO>())
                {
                    if (!subscriberId.StartsWith("Discord:"))
                        continue;

                    yield return new SubscriptionViewModel(ulong.Parse(subscriberId.Remove(0, 8)), nexusModsGameId, nexusModsModId);
                }
            }
        }

        private sealed record SubscriptionDTO(string SubscriberId, uint NexusModsGameId, uint NexusModsModId);
    }
}