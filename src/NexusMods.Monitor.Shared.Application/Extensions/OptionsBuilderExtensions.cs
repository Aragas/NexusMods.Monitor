using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using NexusMods.Monitor.Shared.Application.FluentValidation;

using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Shared.Application.Extensions
{
    public static class OptionsBuilderExtensions
    {
        public static OptionsBuilder<TOptions> ValidateViaFluent<TOptions, TOptionsValidator>(this OptionsBuilder<TOptions> optionsBuilder)
            where TOptions : class
            where TOptionsValidator : class, IValidator<TOptions>
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            optionsBuilder.Services.AddHttpClient("FluentClient")
                .GenerateCorrelationId();
            optionsBuilder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IValidator<TOptions>, TOptionsValidator>());
            optionsBuilder.Services.AddTransient<IValidateOptions<TOptions>>(sp => new FluentValidateOptions<TOptions>(sp.GetRequiredService<IEnumerable<IValidator<TOptions>>>()));

            return optionsBuilder;
        }
    }
}