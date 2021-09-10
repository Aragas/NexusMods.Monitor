using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Metadata.API.RateLimits;
using NexusMods.Monitor.Metadata.Application.Queries.Comments;
using NexusMods.Monitor.Metadata.Application.Queries.Games;
using NexusMods.Monitor.Metadata.Application.Queries.Issues;
using NexusMods.Monitor.Metadata.Application.Queries.Mods;
using NexusMods.Monitor.Metadata.Application.Queries.Threads;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        public record RateLimitViewModel(APILimitViewModel APILimit, SiteLimitViewModel SiteLimit);
        public record APILimitViewModel(int HourlyLimit, int HourlyRemaining, DateTime HourlyReset, int DailyLimit, int DailyRemaining, DateTime DailyReset);
        public record SiteLimitViewModel(DateTimeOffset? RetryAfter);

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
    }
}