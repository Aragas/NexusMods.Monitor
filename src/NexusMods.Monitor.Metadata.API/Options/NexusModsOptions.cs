using FluentValidation;

namespace NexusMods.Monitor.Metadata.API.Options
{
    public sealed class NexusModsOptionsValidator : AbstractValidator<NexusModsOptions>
    {
        public NexusModsOptionsValidator()
        {
            RuleFor(options => options.Endpoint).NotEmpty();
            RuleFor(options => options.APIEndpoint).NotEmpty();
            RuleFor(options => options.APIKey).NotEmpty();
        }
    }

    public sealed record NexusModsOptions
    {
        public string Endpoint { get; init; } = default!;
        public string APIEndpoint { get; init; } = default!;
        public string APIKey { get; init; } = default!;
    }
}