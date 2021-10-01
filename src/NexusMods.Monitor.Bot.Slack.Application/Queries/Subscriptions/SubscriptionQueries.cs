using NexusMods.Monitor.Shared.Application;
using NexusMods.Monitor.Shared.Common;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NexusMods.Monitor.Bot.Slack.Application.Queries.Subscriptions
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
            using var response = await _httpClientFactory.CreateClient("Subscriptions.API").GetAsync("all", ct);
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var contentStream = await response.Content.ReadAsStreamAsync(ct);
                var data = await _jsonSerializer.DeserializeAsync<SubscriptionDTO[]?>(contentStream, ct) ?? Array.Empty<SubscriptionDTO>();
                foreach (var (subscriberId, nexusModsGameId, nexusModsModId, nexusModsGameName, nexusModsModName) in data)
                {
                    if (!subscriberId.StartsWith("Slack:"))
                        continue;

                    yield return new SubscriptionViewModel(subscriberId.Remove(0, 6), nexusModsGameId, nexusModsModId, nexusModsGameName, nexusModsModName);
                }
            }
        }

        private sealed record SubscriptionDTO(string SubscriberId, uint NexusModsGameId, uint NexusModsModId, string NexusModsGameName, string NexusModsModName);
    }
}