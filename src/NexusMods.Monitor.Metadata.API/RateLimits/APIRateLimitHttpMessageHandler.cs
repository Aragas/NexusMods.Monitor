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
        public record APILimit(int HourlyLimit, int HourlyRemaining, DateTime HourlyReset, int DailyLimit, int DailyRemaining, DateTime DailyReset);

        public APILimit APILimitState { get; private set; } = new(0, 0, DateTime.MinValue, 0, 0, DateTime.MinValue);

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

        private TimeLimiter? ParseResponseLimits(HttpResponseMessage response)
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

            APILimitState = new APILimit(
                int.TryParse(hourlyLimitEnum.FirstOrDefault(), out var hourlyLimitVal) ? hourlyLimitVal : 0,
                int.TryParse(hourlyRemainingEnum.FirstOrDefault(), out var hourlyRemainingVal) ? hourlyRemainingVal : 0,
                DateTime.TryParse(hourlyResetEnum.FirstOrDefault(), out var hourlyResetVal) ? hourlyResetVal : DateTime.UtcNow,
                int.TryParse(dailyLimitEnum.FirstOrDefault(), out var dailyLimitVal) ? dailyLimitVal : 0,
                int.TryParse(dailyRemainingEnum.FirstOrDefault(), out var dailyRemainingVal) ? dailyRemainingVal : 0,
                DateTime.TryParse(dailyResetEnum.FirstOrDefault(), out var dailyResetVal) ? dailyResetVal : DateTime.UtcNow
            );

            // A 429 status code can also be served by nginx if the client sends more than 30 requests per second.
            // Nginx will however allow bursts over this for very short periods of time.
            var constraint = new CountByIntervalAwaitableConstraint(30, TimeSpan.FromSeconds(1));

            var hourlyLimitConstraint = new CountByIntervalAwaitableConstraint(APILimitState.HourlyLimit, TimeSpan.FromHours(1));
            var hourlyTimeLeft = APILimitState.HourlyReset - DateTime.UtcNow;
            var hourlyRemainingConstraint = APILimitState.HourlyRemaining <= 0
                ? new BlockUntilDateConstraint(APILimitState.HourlyReset) as IAwaitableConstraint
                : new CountByIntervalAwaitableConstraint(APILimitState.HourlyRemaining, hourlyTimeLeft);

            var dailyLimitConstraint = new CountByIntervalAwaitableConstraint(APILimitState.DailyLimit, TimeSpan.FromDays(1));
            var dailyTimeLeft = APILimitState.DailyReset - DateTime.UtcNow;
            var dailyRemainingConstraint = APILimitState.DailyRemaining <= 0
                ? new BlockUntilDateConstraint(APILimitState.DailyReset) as IAwaitableConstraint
                : new CountByIntervalAwaitableConstraint(APILimitState.DailyRemaining, dailyTimeLeft);

            return TimeLimiter.Compose(constraint, hourlyLimitConstraint, hourlyRemainingConstraint, dailyLimitConstraint, dailyRemainingConstraint);
        }
    }
}