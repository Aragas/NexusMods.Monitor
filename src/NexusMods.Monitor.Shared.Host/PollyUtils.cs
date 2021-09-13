using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Polly;
using Polly.Extensions.Http;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Host
{
    public class PollyUtils
    {
        private static TimeSpan GetServerWaitDuration(DelegateResult<HttpResponseMessage> response)
        {
            if (response.Result?.Headers.RetryAfter is not { } retryAfter)
                return TimeSpan.Zero;

            return retryAfter.Date.HasValue ? retryAfter.Date.Value - DateTime.UtcNow : retryAfter.Delta.GetValueOrDefault(TimeSpan.Zero);
        }

        public static IAsyncPolicy<HttpResponseMessage> PolicySelector(IServiceProvider sp, HttpRequestMessage request)
        {
            var logger = sp.GetRequiredService<ILogger<PollyUtils>>();

            return Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode && r.StatusCode != HttpStatusCode.Unauthorized)
                .OrTransientHttpError()
                .Or<SocketException>()
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: (i, result, context) =>
                    {
                        var clientWaitDuration = TimeSpan.FromSeconds(2);
                        var serverWaitDuration = GetServerWaitDuration(result);
                        var waitDuration = Math.Max(clientWaitDuration.TotalMilliseconds, serverWaitDuration.TotalMilliseconds);
                        return TimeSpan.FromMilliseconds(waitDuration);
                    },
                    onRetryAsync: (result, timeSpan, retryCount, context) =>
                    {
                        logger.LogError(result.Exception, "Exception during HTTP connection. HttpResult {@HttpResult}. Retry count {RetryCount}. Waiting {Time}...", result.Result, retryCount, timeSpan);
                        return Task.CompletedTask;
                    });
        }
    }
}