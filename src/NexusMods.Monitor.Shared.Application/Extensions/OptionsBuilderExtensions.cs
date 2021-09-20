using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;

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

            optionsBuilder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IValidator<TOptions>, TOptionsValidator>());
            optionsBuilder.Validate<IEnumerable<IValidator<TOptions>>>((options, validators) => validators.All(v => v.Validate(options).IsValid));

            return optionsBuilder;
        }
    }
}