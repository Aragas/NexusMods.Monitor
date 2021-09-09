using ComposableAsync;

using RateLimiter;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.API.RateLimits
{
    public class SiteRateLimitHttpMessageHandler : DelegatingHandler
    {
        private readonly TimeLimiter _timeLimiter = TimeLimiter.Compose(
            new CountByIntervalAwaitableConstraint(1, TimeSpan.FromSeconds(1)),
            new CountByIntervalAwaitableConstraint(20, TimeSpan.FromMinutes(1))
        );

        public SiteRateLimitHttpMessageHandler()
        {
            InnerHandler = new SocketsHttpHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            await _timeLimiter;
            return await base.SendAsync(request, ct);
        }
    }
}