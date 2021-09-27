using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Metadata.API.RateLimits;
using NexusMods.Monitor.Metadata.API.Services;
using NexusMods.Monitor.Metadata.Application.Queries.Comments;
using NexusMods.Monitor.Metadata.Application.Queries.Games;
using NexusMods.Monitor.Metadata.Application.Queries.Issues;
using NexusMods.Monitor.Metadata.Application.Queries.Mods;
using NexusMods.Monitor.Metadata.Application.Queries.Threads;
using NexusMods.Monitor.Shared.API.SSE;
using NexusMods.Monitor.Shared.Common;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private record SSORequest([property: JsonPropertyName("id")] Guid Id, [property: JsonPropertyName("token")] string? Token, [property: JsonPropertyName("protocol")] int Protocol);
        private record SSOResponse([property: JsonPropertyName("success")] bool Success, [property: JsonPropertyName("data")] SSOResponseData? Data);
        private record SSOResponseData([property: JsonPropertyName("connection_token")] string? ConnectionToken, [property: JsonPropertyName("api_key")] string? ApiKey);

        public record RateLimitViewModel(APILimitViewModel APILimit, SiteLimitViewModel SiteLimit);
        public record APILimitViewModel(int HourlyLimit, int HourlyRemaining, DateTime HourlyReset, int DailyLimit, int DailyRemaining, DateTime DailyReset);
        public record SiteLimitViewModel(DateTimeOffset? RetryAfter);

        public record AuthorizationStatusResponse(bool IsAuthorized);

        [SuppressMessage("CodeQuality", "IDE0052", Justification = "Reserved for future use")]
        private readonly ILogger<MetadataController> _logger;

        public MetadataController(ILogger<MetadataController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("comments/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<CommentViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public IActionResult GetCommentsAllAsync([FromQuery, BindRequired] uint gameId, [FromQuery, BindRequired] uint modId, [FromServices] ICommentQueries commentQueries, CancellationToken ct) =>
            Ok(commentQueries.GetAllAsync(gameId, modId, ct));

        [HttpGet("issues/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<IssueViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public IActionResult GetIssuesAllAsync([FromQuery, BindRequired] uint gameId, [FromQuery, BindRequired] uint modId, [FromServices] IIssueQueries issueQueries, CancellationToken ct) =>
            Ok(issueQueries.GetAllAsync(gameId, modId, ct));

        [HttpGet("issues/content")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IssueContentViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetIssueContentAsync([FromQuery, BindRequired] uint issueId, [FromServices] IIssueQueries issueQueries, CancellationToken ct) =>
            Ok(await issueQueries.GetContentAsync(issueId, ct));

        [HttpGet("issues/replies")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<IssueReplyViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public IActionResult GetIssueRepliesAsync([FromQuery, BindRequired] uint issueId, [FromServices] IIssueQueries issueQueries, CancellationToken ct) =>
            Ok(issueQueries.GetRepliesAsync(issueId, ct));

        [HttpGet("game/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGameAsync([FromQuery, BindRequired] uint gameId, [FromServices] IGameQueries gameQueries, CancellationToken ct) =>
            Ok(await gameQueries.GetAsync(gameId, ct));

        [HttpGet("game/domain")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGameAsync([FromQuery, BindRequired] string gameDomain, [FromServices] IGameQueries gameQueries, CancellationToken ct) =>
            Ok(await gameQueries.GetAsync(gameDomain, ct));

        [HttpGet("game/all")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<GameViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public IActionResult GetGamesAsync([FromServices] IGameQueries gameQueries, CancellationToken ct) =>
            Ok(gameQueries.GetAllAsync(ct));

        [HttpGet("mod/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetModAsync([FromQuery, BindRequired] uint gameId, [FromQuery, BindRequired] uint modId, [FromServices] IModQueries modQueries, CancellationToken ct) =>
            Ok(await modQueries.GetAsync(gameId, modId, ct));

        [HttpGet("mod/domain")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetModAsync([FromQuery, BindRequired] string gameDomain, [FromQuery, BindRequired] uint modId, [FromServices] IModQueries modQueries, CancellationToken ct) =>
            Ok(await modQueries.GetAsync(gameDomain, modId, ct));

        [HttpGet("thread/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ThreadViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetThreadAsync([FromQuery, BindRequired] uint gameId, [FromQuery, BindRequired] uint modId, [FromServices] IThreadQueries threadQueries, CancellationToken ct) =>
            Ok(await threadQueries.GetAsync(gameId, modId, ct));

        [HttpGet("ratelimits")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RateLimitViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public IActionResult GetRateLimits([FromServices] APIRateLimitHttpMessageHandler apiRateLimit, [FromServices] SiteRateLimitHttpMessageHandler siteRateLimit)
        {
            var (hourlyLimit, hourlyRemaining, hourlyReset, dailyLimit, dailyRemaining, dailyReset) = apiRateLimit.APILimitState;
            var retryAfter = siteRateLimit.APILimitState.RetryAfter;
            return Ok(new RateLimitViewModel(
                new APILimitViewModel(hourlyLimit, hourlyRemaining, hourlyReset, dailyLimit, dailyRemaining, dailyReset),
                new SiteLimitViewModel(retryAfter))
            );
        }

        /// <remarks>
        /// events:
        /// - connecting
        /// - connected
        /// - api-key-failed-to-get
        /// - api-key-received
        /// - finished
        /// </remarks>
        [HttpGet("sso-authorize")]
        [Produces("text/event-stream")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public IActionResult SSOAuthorizeAsync([FromQuery, BindRequired] Guid uuid, [FromServices] NexusModsAPIKeyProvider apiKeyProvider, [FromServices] DefaultJsonSerializer jsonSerializer, CancellationToken ct)
        {
            async IAsyncEnumerable<SSEMessage> SSEEvents()
            {
                var client = new ClientWebSocket();
                using var cts = new CancellationTokenSource(60000);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ct);
                var timeoutToken = linkedCts.Token;
                var connectionToken = null as string;

                yield return new SSEMessage(Event: "connecting");

                var receivedAPIKey = false;
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
                                    receivedAPIKey = true;
                                    apiKeyProvider.Override(response.Data.ApiKey);
                                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye", timeoutToken);
                                }
                            }

                        }
                    }
                }

                await client.ConnectAsync(new Uri("wss://sso.nexusmods.com"), timeoutToken);
                var receiveTask = ReceiveLoop();
                var request = jsonSerializer.SerializeToUtf8Bytes(new SSORequest(uuid, connectionToken, 2));
                await client.SendAsync(request, WebSocketMessageType.Text, true, timeoutToken);

                yield return new SSEMessage(Event: "connected");

                await receiveTask;
                if (!receivedAPIKey)
                    yield return new SSEMessage(Event: "api-key-failed-to-get");
                else
                    yield return new SSEMessage(Event: "api-key-received");

                yield return new SSEMessage(Event: "finished");
            }

            return new SSEActionResult(SSEEvents());
        }

        [HttpGet("authorization-status")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AuthorizationStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthorizationStatusAsync([FromServices] IHttpClientFactory httpClientFactory, CancellationToken ct)
        {
            var response = await httpClientFactory.CreateClient("NexusMods.API").GetAsync("v1/users/validate.json", ct);
            return Ok(new AuthorizationStatusResponse(response.IsSuccessStatusCode));
        }
    }
}