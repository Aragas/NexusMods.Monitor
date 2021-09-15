using Microsoft.Extensions.DependencyInjection;

namespace NexusMods.Monitor.Shared.Host.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddPolly(this IHttpClientBuilder builder) => builder.AddPolicyHandler(PollyUtils.PolicySelector);
    }
}