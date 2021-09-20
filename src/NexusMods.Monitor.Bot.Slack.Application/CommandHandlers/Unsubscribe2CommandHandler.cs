using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Slack.Application.Commands;
using NexusMods.Monitor.Shared.Common;

using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.CommandHandlers
{
    public sealed class Unsubscribe2CommandHandler : IRequestHandler<Unsubscribe2Command, bool>
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public Unsubscribe2CommandHandler(ILogger<Unsubscribe2CommandHandler> logger, IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<bool> Handle(Unsubscribe2Command message, CancellationToken ct)
        {
            var response = await _httpClientFactory.CreateClient("Subscriptions.API").PutAsync("unsubscribe2",
                new StringContent(_jsonSerializer.Serialize(new UnsubscribeDTO($"Slack:{message.ChannelId}", message.NexusModsUrl)), Encoding.UTF8, "application/json"),
                ct);
            return response.IsSuccessStatusCode;
        }

        private sealed record UnsubscribeDTO(string SubscriberId, string NexusModsUrl);
    }
}