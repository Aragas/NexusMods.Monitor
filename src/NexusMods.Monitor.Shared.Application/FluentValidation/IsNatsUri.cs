using FluentValidation;
using FluentValidation.Validators;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusMods.Monitor.Shared.Application.FluentValidation
{
    public interface IIsNatsUriValidator : IPropertyValidator { }

    public class IsNatsUriValidator<T, TProperty> : PropertyValidator<T, TProperty>, IIsNatsUriValidator
    {
        public override string Name => "IsUriValidator";

        public override bool IsValid(ValidationContext<T> context, TProperty value) => value switch
        {
            string s when IsNatsUri(s) => true,
            ICollection<string> c when c.All(IsNatsUri) => true,
            IEnumerable<string> e when e.All(IsNatsUri) => true,
            _ => false
        };

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} is not a NATS uri!";

        private static bool IsNatsUri(string uriString)
        {
            var uri = new Uri(uriString);

            if (!string.Equals(uri.Scheme, "nats", StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (uri.Port != -1 && uri.Port != 4222)
                return false;

            return true;
        }
    }
}
