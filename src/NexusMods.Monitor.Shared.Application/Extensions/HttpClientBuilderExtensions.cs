using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;

namespace NexusMods.Monitor.Shared.Application.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddCorrelationIdOverrideForwarding(this IHttpClientBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof (builder));
            builder.Services.TryAddTransient<CorrelationIdOverrideHandler>();
            builder.AddHttpMessageHandler<CorrelationIdOverrideHandler>();
            return builder;
        }
    }
}