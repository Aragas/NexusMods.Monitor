﻿using FluentValidation;
using FluentValidation.Validators;

using System;

namespace NexusMods.Monitor.Shared.Application.FluentValidation
{
    public interface IIsUriValidator : IPropertyValidator { }

    public class IsUriValidator<T> : PropertyValidator<T, string>, IIsUriValidator
    {
        public override string Name => "IsUriValidator";

        public override bool IsValid(ValidationContext<T> context, string value) => value switch
        {
            { } when string.IsNullOrEmpty(value) => false,
            { } when Uri.TryCreate(value, UriKind.Absolute, out _) => true,
            _ => false
        };

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} is not an uri!";
    }
}