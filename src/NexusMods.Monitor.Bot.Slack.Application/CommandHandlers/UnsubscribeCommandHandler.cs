using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NexusMods.Monitor.Bot.Slack.Application.Commands;
using NexusMods.Monitor.Bot.Slack.Application.Options;
using NexusMods.Monitor.Shared.Application;

using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.CommandHandlers
{
    public sealed class UnsubscribeCommandHandler : IRequestHandler<UnsubscribeCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SubscriptionsOptions _options;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public UnsubscribeCommandHandler(ILogger<UnsubscribeCommandHandler> logger, IHttpClientFactory httpClientFactory, IOptions<SubscriptionsOptions> options, DefaultJsonSerializer jsonSerializer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<bool> Handle(UnsubscribeCommand message, CancellationToken ct)
        {
            var response = await _httpClientFactory.CreateClient().PutAsync(
                $"{_options.APIEndpointV1}/unsubscribe",
                new StringContent(_jsonSerializer.Serialize(new UnsubscribeDTO($"Slack:{message.ChannelId}", message.NexusModsGameId, message.NexusModsModId)), Encoding.UTF8, "application/json"),
                ct);
            return response.IsSuccessStatusCode;
        }

        private sealed record UnsubscribeDTO(string SubscriberId, uint NexusModsGameId, uint NexusModsModId);
    }
}