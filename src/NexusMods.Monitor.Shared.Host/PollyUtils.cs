using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Polly;
using Polly.Extensions.Http;

using System;
using System.Net.Http;
using System.Net.Sockets;

namespace NexusMods.Monitor.Shared.Host
{
    public class PollyUtils
    {
        private static TimeSpan GetServerWaitDuration(DelegateResult<HttpResponseMessage> response)
        {
            var retryAfter = response?.Result?.Headers?.RetryAfter;
            if (retryAfter == null)
                return TimeSpan.Zero;

            return retryAfter.Date.HasValue ? retryAfter.Date.Value - DateTime.UtcNow : retryAfter.Delta.GetValueOrDefault(TimeSpan.Zero);
        }

        public static IAsyncPolicy<HttpResponseMessage> PolicySelector(IServiceProvider sp, HttpRequestMessage request)
        {
            var logger = sp.GetRequiredService<ILogger<PollyUtils>>();

            return Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .OrTransientHttpError()
                .Or<SocketException>()
                .WaitAndRetryAsync(
                    retryCount: 20,
                    sleepDurationProvider: (i, result, context) =>
                    {
                        var clientWaitDuration = TimeSpan.FromSeconds(2);
                        var serverWaitDuration = GetServerWaitDuration(result);
                        var waitDuration = Math.Max(clientWaitDuration.TotalMilliseconds, serverWaitDuration.TotalMilliseconds);
                        return TimeSpan.FromMilliseconds(waitDuration);
                    },
                    onRetryAsync: async (result, timeSpan, retryCount, context) =>
                    {
                        logger.LogError(result.Exception, "Exception during HTTP connection. HttpResult {@HttpResult}. Retry count {RetryCount}. Waiting {Time}...", result.Result, retryCount, timeSpan);
                    });
        }
    }
}