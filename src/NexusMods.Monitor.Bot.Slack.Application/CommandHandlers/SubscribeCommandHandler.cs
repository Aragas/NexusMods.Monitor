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
    public sealed class SubscribeCommandHandler : IRequestHandler<SubscribeCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SubscriptionsOptions _options;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public SubscribeCommandHandler(ILogger<SubscribeCommandHandler> logger, IHttpClientFactory httpClientFactory, IOptions<SubscriptionsOptions> options, DefaultJsonSerializer jsonSerializer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<bool> Handle(SubscribeCommand message, CancellationToken cancellationToken)
        {
            var response = await _httpClientFactory.CreateClient().PutAsync(
                $"{_options.APIEndpointV1}/subscribe",
                new StringContent(_jsonSerializer.Serialize(new SubscribeDTO($"Slack:{message.ChannelId}", message.NexusModsGameId, message.NexusModsModId)), Encoding.UTF8, "application/json"),
                cancellationToken);
            return response.IsSuccessStatusCode;
        }

        private sealed record SubscribeDTO(string SubscriberId, uint NexusModsGameId, uint NexusModsModId);
    }
}