using FluentValidation;
using FluentValidation.Validators;

using Polly;
using Polly.Retry;

using System;
using System.Net.Http;
using System.Threading;

namespace NexusMods.Monitor.Shared.Application.FluentValidation
{
    public interface IIsUriAvailableValidator : IPropertyValidator { }

    public class IsUriAvailableValidator<T> : PropertyValidator<T, string>, IIsUriAvailableValidator
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RetryPolicy _policy;

        public override string Name => "IsUriAvailableValidator";

        public IsUriAvailableValidator(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

            _policy = Policy
                .Handle<Exception>(exception => true)
                .WaitAndRetry(
                    retryCount: 20,
                    sleepDurationProvider: (i, result, context) => TimeSpan.FromSeconds(2),
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        return;
                    });
        }

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            var result = _policy.ExecuteAndCapture(() =>
            {
                var client = _httpClientFactory.CreateClient("FluentClient");
                var request = new HttpRequestMessage(HttpMethod.Options, value);
                using var cts = new CancellationTokenSource(2000);
                var response = client.Send(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            });

            return result.Outcome == OutcomeType.Successful;
        }

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} has an invalid uri! Message:\n{RequestException}";
    }
}