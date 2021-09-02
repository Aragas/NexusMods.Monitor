using Microsoft.Extensions.Options;

using NexusMods.Monitor.Scraper.Application.Options;

using NexusModsNET;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application
{
    public class NexusModsClientWrapper : INexusModsClient
    {
        public string APIKey => _implementation.APIKey;
        public string ProductName => _implementation.ProductName;
        public string ProductVersion => _implementation.ProductVersion;
        public IRateLimitsManagement RateLimitsManagement => _implementation.RateLimitsManagement;
        public string UserAgent => _implementation.UserAgent;

        private readonly INexusModsClient _implementation;

        public NexusModsClientWrapper(IOptions<NexusModsOptions> options)
        {
            _implementation = NexusModsClient.Create(options.Value.APIKey);
        }

        public HttpRequestMessage ConstructHttpRequestMessage(Uri requestURI, HttpMethod method, HttpContent? httpContent = null, string? acceptedMediaType = null) =>
            _implementation.ConstructHttpRequestMessage(requestURI, method, httpContent, acceptedMediaType);

        public Task<T> ProcessRequestAsync<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default) =>
            _implementation.ProcessRequestAsync<T>(requestMessage, cancellationToken);

        public Task<T> ProcessRequestAsync<T>(Uri requestURI, HttpMethod method, CancellationToken cancellationToken = default, HttpContent? formData = null) =>
            _implementation.ProcessRequestAsync<T>(requestURI, method, cancellationToken, formData);

        public void Dispose()
        {
            _implementation.Dispose();
        }
    }
}