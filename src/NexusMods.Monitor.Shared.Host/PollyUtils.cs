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
        public static IAsyncPolicy<HttpResponseMessage> PolicySelector(IServiceProvider sp, HttpRequestMessage request)
        {
            var logger = sp.GetRequiredService<ILogger<PollyUtils>>();
            return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .OrTransientHttpError()
                .Or<SocketException>()
                .WaitAndRetryAsync(20, _ => TimeSpan.FromSeconds(2), (delegateResult, time) =>
                {
                    logger.LogError(delegateResult.Exception, "Exception during HTTP connection. HttpResult {@HttpResult}. Waiting {Time}...", time, delegateResult.Result);
                });
        }
    }
}