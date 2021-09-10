using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Metadata.API.RateLimits;
using NexusMods.Monitor.Metadata.Application.Queries.Comments;
using NexusMods.Monitor.Metadata.Application.Queries.Games;
using NexusMods.Monitor.Metadata.Application.Queries.Issues;
using NexusMods.Monitor.Metadata.Application.Queries.Mods;
using NexusMods.Monitor.Metadata.Application.Queries.Threads;
using NexusMods.Monitor.Shared.Application;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        public record SSOAuthorizeRequest(Guid Id);
        private record SSORequest([property: JsonPropertyName("id")] Guid Id, [property: JsonPropertyName("token")] string? Token, [property: JsonPropertyName("protocol")] int Protocol);
        private record SSOResponse([property: JsonPropertyName("success")] bool Success, [property: JsonPropertyName("data")] SSOResponseData? Data);
        private record SSOResponseData([property: JsonPropertyName("connection_token")] string? ConnectionToken, [property: JsonPropertyName("api_key")] string? ApiKey);

        public record RateLimitViewModel(APILimitViewModel APILimit, SiteLimitViewModel SiteLimit);
        public record APILimitViewModel(int HourlyLimit, int HourlyRemaining, DateTime HourlyReset, int DailyLimit, int DailyRemaining, DateTime DailyReset);
        public record SiteLimitViewModel(DateTimeOffset? RetryAfter);

        public record AuthorizationStatusResponse(bool IsAuthorized);

        private readonly ILogger<MetadataController> _logger;

        public MetadataController(ILogger<MetadataController> logger, IGameQueries gameQueries)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("comments/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<CommentViewModel>), StatusCodes.Status200OK)]
        public IActionResult GetCommentsAllAsync(uint gameId, uint modId, CancellationToken ct, [FromServices] ICommentQueries commentQueries) => Ok(commentQueries.GetAllAsync(gameId, modId, ct));

        [HttpGet("issues/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<IssueViewModel>), StatusCodes.Status200OK)]
        public IActionResult GetIssuesAllAsync(uint gameId, uint modId, CancellationToken ct, [FromServices] IIssueQueries issueQueries) => Ok(issueQueries.GetAllAsync(gameId, modId, ct));

        [HttpGet("issues/content")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IssueContentViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIssueContentAsync(uint issueId, CancellationToken ct, [FromServices] IIssueQueries issueQueries) => Ok(await issueQueries.GetContentAsync(issueId, ct));

        [HttpGet("issues/replies")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<IssueReplyViewModel>), StatusCodes.Status200OK)]
        public IActionResult GetIssueRepliesAsync(uint issueId, CancellationToken ct, [FromServices] IIssueQueries issueQueries) => Ok(issueQueries.GetRepliesAsync(issueId, ct));

        [HttpGet("game/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGameAsync(uint gameId, CancellationToken ct, [FromServices] IGameQueries gameQueries) => Ok(await gameQueries.GetAsync(gameId, ct));

        [HttpGet("game/domain")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGameAsync(string gameDomain, CancellationToken ct, [FromServices] IGameQueries gameQueries) => Ok(await gameQueries.GetAsync(gameDomain, ct));

        [HttpGet("game/all")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<GameViewModel>), StatusCodes.Status200OK)]
        public IActionResult GetGamesAsync(CancellationToken ct, [FromServices] IGameQueries gameQueries) => Ok(gameQueries.GetAllAsync(ct));

        [HttpGet("mod/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetModAsync(uint gameId, uint modId, CancellationToken ct, [FromServices] IModQueries modQueries) => Ok(await modQueries.GetAsync(gameId, modId, ct));

        [HttpGet("mod/domain")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetModAsync(string gameDomain, uint modId, CancellationToken ct, [FromServices] IModQueries modQueries) => Ok(await modQueries.GetAsync(gameDomain, modId, ct));

        [HttpGet("thread/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ThreadViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetThreadAsync(uint gameId, uint modId, CancellationToken ct, [FromServices] IThreadQueries threadQueries) => Ok(await threadQueries.GetAsync(gameId, modId, ct));

        [HttpGet("ratelimits")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RateLimitViewModel), StatusCodes.Status200OK)]
        public IActionResult GetRateLimits([FromServices] APIRateLimitHttpMessageHandler apiRateLimit, [FromServices] SiteRateLimitHttpMessageHandler siteRateLimit)
        {
            var (hourlyLimit, hourlyRemaining, hourlyReset, dailyLimit, dailyRemaining, dailyReset) = apiRateLimit.APILimitState;
            var retryAfter = siteRateLimit.APILimitState.RetryAfter;
            return Ok(new RateLimitViewModel(
                new APILimitViewModel(hourlyLimit, hourlyRemaining, hourlyReset, dailyLimit, dailyRemaining, dailyReset),
                new SiteLimitViewModel(retryAfter))
            );
        }

        [HttpPost("sso-authorize")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> SSOAuthorizeAsync([FromBody] SSOAuthorizeRequest authorizeRequest, CancellationToken ct, [FromServices] NexusModsAPIKeyProvider apiKeyProvider, [FromServices] DefaultJsonSerializer jsonSerializer)
        {
            var client = new ClientWebSocket();
            var timeoutToken = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource(60000).Token, ct).Token;
            var connectionToken = null as string;

            async Task ReceiveLoop()
            {
                var buffer = new ArraySegment<byte>(new byte[1024]);
                var sb = new StringBuilder();
                while (!timeoutToken.IsCancellationRequested && client.State == WebSocketState.Open)
                {
                    var received = await client.ReceiveAsync(buffer, timeoutToken);
                    if (received.MessageType == WebSocketMessageType.Close)
                        break;

                    sb.Append(Encoding.UTF8.GetString(buffer.AsSpan(0, received.Count)));
                    if (received.EndOfMessage)
                    {
                        var text = sb.ToString();
                        sb.Clear();

                        var response = jsonSerializer.Deserialize<SSOResponse?>(text);
                        if (response is not null && response.Success && response.Data is not null)
                        {
                            if (response.Data.ConnectionToken is not null)
                            {
                                connectionToken = response.Data.ConnectionToken;
                            }

                            if (response.Data.ApiKey is not null)
                            {
                                apiKeyProvider.Override(response.Data.ApiKey);
                                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye", timeoutToken);
                            }
                        }

                    }
                }
            }

            await client.ConnectAsync(new Uri("wss://sso.nexusmods.com"), timeoutToken);
            var receiveTask = ReceiveLoop();
            var request = jsonSerializer.SerializeToUtf8Bytes(new SSORequest(authorizeRequest.Id, connectionToken, 2));
            await client.SendAsync(request, WebSocketMessageType.Text, true, timeoutToken);
            await receiveTask;

            return Ok();
        }

        [HttpPost("authorization-status")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuthorizationStatusAsync(CancellationToken ct, [FromServices] IHttpClientFactory httpClientFactory, [FromServices] DefaultJsonSerializer jsonSerializer)
        {
            var response = await httpClientFactory.CreateClient("NexusMods.API").GetAsync("v1/users/validate.json", ct);
            return Ok(new AuthorizationStatusResponse(response.IsSuccessStatusCode));
        }
    }
}