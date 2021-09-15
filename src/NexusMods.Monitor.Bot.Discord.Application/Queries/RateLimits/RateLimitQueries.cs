using NexusMods.Monitor.Shared.Application;
using NexusMods.Monitor.Shared.Common;

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.Queries.RateLimits
{
    public sealed class RateLimitQueries : IRateLimitQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public RateLimitQueries(IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<RateLimitViewModel?> GetAsync(CancellationToken ct = default)
        {
            using var response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync("ratelimits", ct);
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                if (_jsonSerializer.Deserialize<RateLimitDTO?>(content) is { } tuple)
                {
                    var ((hourlyLimit, hourlyRemaining, hourlyReset, dailyLimit, dailyRemaining, dailyReset), siteLimitDTO) = tuple;
                    return new RateLimitViewModel(new APILimitViewModel(hourlyLimit, hourlyRemaining, hourlyReset, dailyLimit, dailyRemaining, dailyReset), new SiteLimitViewModel(siteLimitDTO.RetryAfter));
                }
            }
            return null;
        }

        public record RateLimitDTO(APILimitDTO APILimit, SiteLimitDTO SiteLimit);
        public record APILimitDTO(int HourlyLimit, int HourlyRemaining, DateTime HourlyReset, int DailyLimit, int DailyRemaining, DateTime DailyReset);
        public record SiteLimitDTO(DateTimeOffset? RetryAfter);
    }
}