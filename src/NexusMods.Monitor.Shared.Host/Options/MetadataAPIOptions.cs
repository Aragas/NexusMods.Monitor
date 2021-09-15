using FluentValidation;

namespace NexusMods.Monitor.Shared.Host.Options
{
    public sealed class MetadataAPIOptionsValidator : AbstractValidator<MetadataAPIOptions>
    {
        public MetadataAPIOptionsValidator()
        {
            RuleFor(options => options.APIEndpointV1).NotEmpty();
        }
    }

    public sealed record MetadataAPIOptions
    {
        public string APIEndpointV1 { get; init; } = default!;
    }
}