﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NexusMods.Monitor.Shared.Host.Options;

using System;
using System.Diagnostics.CodeAnalysis;

namespace NexusMods.Monitor.Shared.Host.Extensions
{
    /// <summary>
    /// Extension methods for adding configuration related options services to the DI container via <see cref="OptionsBuilder{TOptions}"/>.
    /// </summary>
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2091", Justification = "Workaround for https://github.com/mono/linker/issues/1416. Outer method has been annotated with DynamicallyAccessedMembers.")]
    public static class OptionsBuilderExtensions
    {
        /// <summary>
        /// Enforces options validation check on start rather than in runtime.
        /// </summary>
        /// <typeparam name="TOptions">The type of options.</typeparam>
        /// <param name="optionsBuilder">The <see cref="OptionsBuilder{TOptions}"/> to configure options instance.</param>
        /// <returns>The <see cref="OptionsBuilder{TOptions}"/> so that additional calls can be chained.</returns>
        public static OptionsBuilder<TOptions> ValidateViaHostManager<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions>(this OptionsBuilder<TOptions> optionsBuilder)
            where TOptions : class
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            optionsBuilder.Services.AddOptions<ValidatorOptions>().Configure<IOptionsMonitor<TOptions>>((vo, options) =>
            {
                // This adds an action that resolves the options value to force evaluation
                // We don't care about the result as duplicates are not important
                vo.Validators[typeof(TOptions)] = () => options.Get(optionsBuilder.Name);
            });

            return optionsBuilder;
        }
    }
}