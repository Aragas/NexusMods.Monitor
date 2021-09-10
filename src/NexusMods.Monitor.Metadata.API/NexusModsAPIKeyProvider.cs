using Microsoft.Extensions.Options;

using NexusMods.Monitor.Metadata.API.Options;

namespace NexusMods.Monitor.Metadata.API
{
    public sealed class NexusModsAPIKeyProvider
    {
        private readonly NexusModsOptions _options;
        private string? _apiKey;

        public NexusModsAPIKeyProvider(IOptions<NexusModsOptions> options)
        {
            _options = options.Value;
        }

        public string Get() => _apiKey ?? _options.APIKey;

        public void Override(string? apiKey) => _apiKey = apiKey;
    }
}