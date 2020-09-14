using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using NexusMods.Monitor.Bot.Slack.Application.Commands;
using NexusMods.Monitor.Bot.Slack.Application.Options;

using System;
using System.Net.Http;
using System.Runtime.Serialization;
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

        public SubscribeCommandHandler(ILogger<SubscribeCommandHandler> logger, IHttpClientFactory httpClientFactory, IOptions<SubscriptionsOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<bool> Handle(SubscribeCommand message, CancellationToken cancellationToken)
        {
            var response = await _httpClientFactory.CreateClient().PutAsync(
                $"{_options.APIEndpointV1}/subscribe",
                new StringContent(JsonConvert.SerializeObject(new SubscribeDTO($"Slack:{message.ChannelId}", message.NexusModsGameId, message.NexusModsModId)), Encoding.UTF8, "application/json"),
                cancellationToken);
            return response.IsSuccessStatusCode;
        }

        [DataContract]
        private sealed class SubscribeDTO
        {
            [DataMember(Name = "subscriberId")]
            public string SubscriberId { get; private set; } = default!;
            [DataMember(Name = "nexusModsGameId")]
            public uint NexusModsGameId { get; private set; } = default!;
            [DataMember(Name = "nexusModsModId")]
            public uint NexusModsModId { get; private set;} = default!;

            private SubscribeDTO() { }
            public SubscribeDTO(string subscriberId, uint nexusModsGameId, uint nexusModsModId)
            {
                SubscriberId = subscriberId;
                NexusModsGameId = nexusModsGameId;
                NexusModsModId = nexusModsModId;
            }
        }
    }
}