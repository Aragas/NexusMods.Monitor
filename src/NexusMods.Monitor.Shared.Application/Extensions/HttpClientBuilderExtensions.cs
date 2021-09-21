using CorrelationId;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using System;

namespace NexusMods.Monitor.Shared.Application.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder GenerateCorrelationId(this IHttpClientBuilder builder) => builder
            .ConfigureHttpClient((sp, client) =>
            {
                var correlationIdOptions = sp.GetRequiredService<IOptions<CorrelationIdOptions>>().Value;
                client.DefaultRequestHeaders.Add(correlationIdOptions.RequestHeader, Guid.NewGuid().ToString());
            });

        public static IHttpClientBuilder AddCorrelationIdOverrideForwarding(this IHttpClientBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.TryAddTransient<CorrelationIdOverrideHandler>();
            builder.AddHttpMessageHandler<CorrelationIdOverrideHandler>();
            return builder;
        }
    }
}