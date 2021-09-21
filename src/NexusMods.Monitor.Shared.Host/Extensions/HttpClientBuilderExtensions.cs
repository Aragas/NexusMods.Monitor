using Microsoft.Extensions.DependencyInjection;

using System;

namespace NexusMods.Monitor.Shared.Host.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddPolly(this IHttpClientBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddPolicyHandler(PollyUtils.PolicySelector);
        }
    }
}