using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Application.Commands;
using NexusMods.Monitor.Shared.Application.SSE;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Application.CommandHandlers
{
    public sealed class SSOAuthorizeCommandHandler : IRequestHandler<SSOAuthorizeCommand, ISSOAuthorizationHandler>
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public SSOAuthorizeCommandHandler(ILogger<SSOAuthorizeCommandHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<ISSOAuthorizationHandler> Handle(SSOAuthorizeCommand message, CancellationToken ct)
        {
            return await new SSOAuthorizationHandler(message.Id, _httpClientFactory).StartAsync(ct);
        }
    }
}