using ComposableAsync;

using RateLimiter;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.API.RateLimits
{
    public class APIRateLimitHttpMessageHandler : DelegatingHandler
    {
        private readonly SemaphoreSlim _lock = new(1, 1);
        private TimeLimiter _timeLimiter = TimeLimiter.GetFromMaxCountByInterval(30, TimeSpan.FromSeconds(1));

        public APIRateLimitHttpMessageHandler()
        {
            InnerHandler = new SocketsHttpHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            try
            {
                await _lock.WaitAsync(ct);
                await _timeLimiter;

                var response = await base.SendAsync(request, ct);
                if (response.IsSuccessStatusCode && ParseResponseLimits(response) is { } timeLimiter)
                {
                    _timeLimiter = timeLimiter;
                }
                return response;
            }
            finally
            {
                _lock.Release();
            }
        }

        private static TimeLimiter? ParseResponseLimits(HttpResponseMessage response)
        {
            if (!response.Headers.TryGetValues("X-RL-Hourly-Limit", out var hourlyLimitEnum))
                return null;
            if (!response.Headers.TryGetValues("X-RL-Hourly-Remaining", out var hourlyRemainingEnum))
                return null;
            if (!response.Headers.TryGetValues("X-RL-Hourly-Reset", out var hourlyResetEnum))
                return null;
            if (!response.Headers.TryGetValues("X-RL-Daily-Limit", out var dailyLimitEnum))
                return null;
            if (!response.Headers.TryGetValues("X-RL-Daily-Remaining", out var dailyRemainingEnum))
                return null;
            if (!response.Headers.TryGetValues("X-RL-Daily-Reset", out var dailyResetEnum))
                return null;


            // A 429 status code can also be served by nginx if the client sends more than 30 requests per second.
            // Nginx will however allow bursts over this for very short periods of time.
            var constraint = new CountByIntervalAwaitableConstraint(30, TimeSpan.FromSeconds(1));


            var hourlyLimit = int.TryParse(hourlyLimitEnum.FirstOrDefault(), out var hourlyLimitVal) ? hourlyLimitVal : 0;
            var hourlyLimitConstraint = new CountByIntervalAwaitableConstraint(hourlyLimit, TimeSpan.FromHours(1));

            var hourlyRemaining = int.TryParse(hourlyRemainingEnum.FirstOrDefault(), out var hourlyRemainingVal) ? hourlyRemainingVal : 0;
            var hourlyReset = DateTime.TryParse(hourlyResetEnum.FirstOrDefault(), out var hourlyResetVal) ? hourlyResetVal : DateTime.UtcNow;
            var hourlyTimeLeft = hourlyReset - DateTime.UtcNow;
            var hourlyRemainingConstraint = hourlyRemaining <= 0
                ? (IAwaitableConstraint) new BlockUntilDateConstraint(hourlyReset)
                : (IAwaitableConstraint) new CountByIntervalAwaitableConstraint(hourlyRemaining, hourlyTimeLeft);


            var dailyLimit = int.TryParse(dailyLimitEnum.FirstOrDefault(), out var dailyLimitVal) ? dailyLimitVal : 0;
            var dailyLimitConstraint = new CountByIntervalAwaitableConstraint(dailyLimit, TimeSpan.FromDays(1));

            var dailyRemaining = int.TryParse(dailyRemainingEnum.FirstOrDefault(), out var dailyRemainingVal) ? dailyRemainingVal : 0;
            var dailyReset = DateTime.TryParse(dailyResetEnum.FirstOrDefault(), out var dailyResetVal) ? dailyResetVal : DateTime.UtcNow;
            var dailyTimeLeft = dailyReset - DateTime.UtcNow;
            var dailyRemainingConstraint = dailyRemaining <= 0
                ? (IAwaitableConstraint) new BlockUntilDateConstraint(dailyReset)
                : (IAwaitableConstraint) new CountByIntervalAwaitableConstraint(dailyRemaining, dailyTimeLeft);


            return TimeLimiter.Compose(constraint, hourlyLimitConstraint, hourlyRemainingConstraint, dailyLimitConstraint, dailyRemainingConstraint);
        }
    }
}