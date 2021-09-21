using FluentValidation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NexusMods.Monitor.Shared.Application.Extensions;

using System;

namespace NexusMods.Monitor.Shared.Host.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValidatedOptions<TOptions, TOptionsValidator>(this IServiceCollection services, IConfiguration configuration)
            where TOptions : class where TOptionsValidator : class, IValidator<TOptions>
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddOptions<TOptions>()
                .Bind(configuration)
                .ValidateViaFluent<TOptions, TOptionsValidator>()
                .ValidateViaHostManager();

            return services;
        }
    }
}