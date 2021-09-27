using FluentValidation;
using FluentValidation.Validators;

using NexusMods.Monitor.Shared.Application.Extensions;

using System;
using System.Net.Http;
using System.Threading;

namespace NexusMods.Monitor.Metadata.API.Options
{
    internal class NexusModsTokenValidator : PropertyValidator<NexusModsOptions, string>
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public override string Name => "NexusModsTokenValidator";

        public NexusModsTokenValidator(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public override bool IsValid(ValidationContext<NexusModsOptions> context, string value)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("FluentClient");
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(new Uri(context.InstanceToValidate.APIEndpoint), "v1/users/validate.json"));
                request.Headers.Add("apikey", value);
                using var cts = new CancellationTokenSource(2000);
                var response = client.Send(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                if (!response.IsSuccessStatusCode)
                {
                    context.MessageFormatter.AppendArgument("RequestException", $"Status code: {response.StatusCode}");
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                context.MessageFormatter.AppendArgument("RequestException", e);
                return false;
            }
        }

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} has an invalid ApiKey! Message:\n{RequestException}";
    }

    public sealed class NexusModsOptionsValidator : AbstractValidator<NexusModsOptions>
    {
        public NexusModsOptionsValidator(IHttpClientFactory httpClientFactory)
        {
            RuleFor(options => options.APIKey).NotEmpty().DependentRules(() =>
                    RuleFor(options => options.APIEndpoint).IsUri().IsUriAvailable(httpClientFactory))
                .SetValidator(new NexusModsTokenValidator(httpClientFactory));

            RuleFor(options => options.Endpoint).IsUri().IsUriAvailable(httpClientFactory);

        }
    }

    public sealed record NexusModsOptions
    {
        public string Endpoint { get; init; } = default!;
        public string APIEndpoint { get; init; } = default!;
        public string APIKey { get; init; } = default!;
    }
}