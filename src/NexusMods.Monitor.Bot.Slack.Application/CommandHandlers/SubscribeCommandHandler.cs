﻿using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Slack.Application.Commands;
using NexusMods.Monitor.Shared.Common;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.CommandHandlers
{
    public sealed class SubscribeCommandHandler : IRequestHandler<SubscribeCommand, bool>
    {
        [SuppressMessage("CodeQuality", "IDE0052", Justification = "Reserved for future use")]
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public SubscribeCommandHandler(ILogger<SubscribeCommandHandler> logger, IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<bool> Handle(SubscribeCommand message, CancellationToken ct)
        {
            var response = await _httpClientFactory.CreateClient("Subscriptions.API").PutAsync("subscribe",
                new StringContent(_jsonSerializer.Serialize(new SubscribeDTO($"Slack:{message.ChannelId}", message.NexusModsGameId, message.NexusModsModId)), Encoding.UTF8, "application/json"),
                ct);
            return response.IsSuccessStatusCode;
        }

        private sealed record SubscribeDTO(string SubscriberId, uint NexusModsGameId, uint NexusModsModId);
    }
}