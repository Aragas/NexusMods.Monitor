﻿using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using NexusMods.Monitor.Bot.Discord.Application.Commands;
using NexusMods.Monitor.Bot.Discord.Application.Options;

using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.CommandHandlers
{
    public sealed class UnsubscribeCommandHandler : IRequestHandler<UnsubscribeCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SubscriptionsOptions _options;

        public UnsubscribeCommandHandler(ILogger<UnsubscribeCommandHandler> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<SubscriptionsOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<bool> Handle(UnsubscribeCommand message, CancellationToken cancellationToken)
        {
            var response = await _httpClientFactory.CreateClient().PutAsync(
                $"{_options.APIEndpointV1}/unsubscribe",
                new StringContent(JsonConvert.SerializeObject(new UnsubscribeDTO($"Discord:{message.ChannelId}", message.NexusModsGameId, message.NexusModsModId)), Encoding.UTF8, "application/json"),
                cancellationToken);
            return response.IsSuccessStatusCode;
        }

        [DataContract]
        private sealed class UnsubscribeDTO
        {
            [DataMember(Name = "subscriberId")]
            public string SubscriberId { get; private set; } = default!;
            [DataMember(Name = "nexusModsGameId")]
            public uint NexusModsGameId { get; private set;} = default!;
            [DataMember(Name = "nexusModsModId")]
            public uint NexusModsModId { get; private set;} = default!;

            private UnsubscribeDTO() { }
            public UnsubscribeDTO(string subscriberId, uint nexusModsGameId, uint nexusModsModId)
            {
                SubscriberId = subscriberId;
                NexusModsGameId = nexusModsGameId;
                NexusModsModId = nexusModsModId;
            }
        }
    }
}