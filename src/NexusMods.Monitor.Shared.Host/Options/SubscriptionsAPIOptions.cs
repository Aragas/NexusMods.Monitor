using FluentValidation;

namespace NexusMods.Monitor.Shared.Host.Options
{
    public sealed class SubscriptionsAPIOptionsValidator : AbstractValidator<SubscriptionsAPIOptions>
    {
        public SubscriptionsAPIOptionsValidator()
        {
            RuleFor(options => options.APIEndpointV1).NotEmpty();
        }
    }

    public sealed record SubscriptionsAPIOptions
    {
        public string APIEndpointV1 { get; set; } = default!;
    }
}