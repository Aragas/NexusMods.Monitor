using Microsoft.Extensions.Options;

using NexusMods.Monitor.Metadata.API.Options;

using NexusModsNET;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.API
{
    internal class NexusModsClientWrapper : INexusModsClient
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

        public HttpRequestMessage ConstructHttpRequestMessage(Uri requestUri, HttpMethod method, HttpContent? httpContent = null, string? acceptedMediaType = null) =>
            _implementation.ConstructHttpRequestMessage(requestUri, method, httpContent, acceptedMediaType);

        public Task<T> ProcessRequestAsync<T>(HttpRequestMessage requestMessage, CancellationToken ct = default) =>
            _implementation.ProcessRequestAsync<T>(requestMessage, ct);

        public Task<T> ProcessRequestAsync<T>(Uri requestUri, HttpMethod method, CancellationToken ct = default, HttpContent? formData = null) =>
            _implementation.ProcessRequestAsync<T>(requestUri, method, ct, formData);

        public void Dispose()
        {
            _implementation.Dispose();
        }
    }
}