using FluentValidation;

using NexusMods.Monitor.Shared.Application.Extensions;

using System.Net.Http;

namespace NexusMods.Monitor.Shared.Host.Options
{
    public sealed class MetadataAPIOptionsValidator : AbstractValidator<MetadataAPIOptions>
    {
        public MetadataAPIOptionsValidator(IHttpClientFactory httpClientFactory)
        {
            RuleFor(options => options.APIEndpointV1).IsUri().IsUriAvailable(httpClientFactory);
        }
    }

    public sealed record MetadataAPIOptions
    {
        public string APIEndpointV1 { get; init; } = default!;
    }
}