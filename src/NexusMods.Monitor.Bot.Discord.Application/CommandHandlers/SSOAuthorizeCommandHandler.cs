using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Application.Commands;
using NexusMods.Monitor.Shared.Application;

using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.CommandHandlers
{
    public sealed class SSOAuthorizeCommandHandler : IRequestHandler<SSOAuthorizeCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public SSOAuthorizeCommandHandler(ILogger<SSOAuthorizeCommandHandler> logger, IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<bool> Handle(SSOAuthorizeCommand message, CancellationToken ct)
        {
            var response = await _httpClientFactory.CreateClient("Metadata.API").PostAsync(
                "sso-authorize",
                new StringContent(_jsonSerializer.Serialize(new SSOAuthorizeDTO(message.Id)), Encoding.UTF8, "application/json"),
                ct);
            return response.IsSuccessStatusCode;
        }

        private sealed record SSOAuthorizeDTO(Guid Id);
    }
}