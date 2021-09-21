using FluentValidation;

using NexusMods.Monitor.Shared.Application.FluentValidation;

using System;
using System.Net.Http;

namespace NexusMods.Monitor.Shared.Application.Extensions
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<T, string> NotInteger<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            if (ruleBuilder == null)
            {
                throw new ArgumentNullException(nameof(ruleBuilder));
            }

            return ruleBuilder.SetValidator(new NotIntegerValidator<T>());
        }

        public static IRuleBuilderOptions<T, string> NotBoolean<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            if (ruleBuilder == null)
            {
                throw new ArgumentNullException(nameof(ruleBuilder));
            }

            return ruleBuilder.SetValidator(new NotBooleanValidator<T>());
        }

        public static IRuleBuilderOptions<T, string> IsUri<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            if (ruleBuilder == null)
            {
                throw new ArgumentNullException(nameof(ruleBuilder));
            }

            return ruleBuilder.SetValidator(new IsUriValidator<T>());
        }

        public static IRuleBuilderOptions<T, string> IsUriAvailable<T>(this IRuleBuilder<T, string> ruleBuilder, IHttpClientFactory httpClientFactory)
        {
            if (ruleBuilder == null)
            {
                throw new ArgumentNullException(nameof(ruleBuilder));
            }

            return ruleBuilder.SetValidator(new IsUriAvailableValidator<T>(httpClientFactory));
        }

        public static IRuleBuilderOptions<T, TProperty> IsNatsUri<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            if (ruleBuilder == null)
            {
                throw new ArgumentNullException(nameof(ruleBuilder));
            }

            return ruleBuilder.SetValidator(new IsNatsUriValidator<T, TProperty>());
        }
    }
}