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
        public string Endpoint { get; set; } = default!;
        public string APIEndpoint { get; set; } = default!;
        public string APIKey { get; set; } = default!;
    }
}