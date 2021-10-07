using CorrelationId;
using CorrelationId.Abstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;

namespace NexusMods.Monitor.Shared.Application.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder GenerateCorrelationId(this IHttpClientBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureHttpClient((sp, client) =>
            {
                var correlationIdOptions = sp.GetRequiredService<IOptions<CorrelationIdOptions>>().Value;
                var correlationIdProvider = sp.GetRequiredService<ICorrelationIdProvider>();
                client.DefaultRequestHeaders.Add(correlationIdOptions.RequestHeader, correlationIdProvider.GenerateCorrelationId(null));
            });

            return builder;
        }

        public static IHttpClientBuilder AddCorrelationIdOverrideForwarding(this IHttpClientBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.TryAddTransient<CorrelationIdOverrideHandler>();
            builder.AddHttpMessageHandler<CorrelationIdOverrideHandler>();
            return builder;
        }
    }
}