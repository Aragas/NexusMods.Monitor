using FluentValidation;

using NexusMods.Monitor.Shared.Application.Extensions;

using System.Net.Http;

namespace NexusMods.Monitor.Shared.Host.Options
{
    public sealed class SubscriptionsAPIOptionsValidator : AbstractValidator<SubscriptionsAPIOptions>
    {
        public SubscriptionsAPIOptionsValidator(IHttpClientFactory httpClientFactory)
        {
            RuleFor(options => options.APIEndpointV1).IsUri().IsUriAvailable(httpClientFactory);
        }
    }

    public sealed record SubscriptionsAPIOptions
    {
        public string APIEndpointV1 { get; init; } = default!;
    }
}