using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<MetadataController> _logger;
        private readonly ICommentQueries _commentQueries;
        private readonly IIssueQueries _issueQueries;
        private readonly IGameQueries _gameQueries;
        private readonly IModQueries _modQueries;
        private readonly IThreadQueries _threadQueries;

        public MetadataController(ILogger<MetadataController> logger, ICommentQueries commentQueries, IIssueQueries issueQueries, IGameQueries gameQueries, IModQueries modQueries, IThreadQueries threadQueries)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commentQueries = commentQueries ?? throw new ArgumentNullException(nameof(commentQueries));
            _issueQueries = issueQueries ?? throw new ArgumentNullException(nameof(issueQueries));
            _gameQueries = gameQueries ?? throw new ArgumentNullException(nameof(gameQueries));
            _modQueries = modQueries ?? throw new ArgumentNullException(nameof(modQueries));
            _threadQueries = threadQueries ?? throw new ArgumentNullException(nameof(threadQueries));
        }

        [HttpGet("comments/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<CommentViewModel>), StatusCodes.Status200OK)]
        public IActionResult GetCommentsAllAsync(uint gameId, uint modId, CancellationToken ct) => Ok(_commentQueries.GetAllAsync(gameId, modId, ct));

        [HttpGet("issues/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<IssueViewModel>), StatusCodes.Status200OK)]
        public IActionResult GetIssuesAllAsync(uint gameId, uint modId, CancellationToken ct) => Ok(_issueQueries.GetAllAsync(gameId, modId, ct));

        [HttpGet("issues/content")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IssueContentViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIssueContentAsync(uint issueId, CancellationToken ct) => Ok(await _issueQueries.GetContentAsync(issueId, ct));

        [HttpGet("issues/replies")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<IssueReplyViewModel>), StatusCodes.Status200OK)]
        public IActionResult GetIssueRepliesAsync(uint issueId, CancellationToken ct) => Ok(_issueQueries.GetRepliesAsync(issueId, ct));

        [HttpGet("game/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGameAsync(uint gameId, CancellationToken ct) => Ok(await _gameQueries.GetAsync(gameId, ct));

        [HttpGet("game/domain")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGameAsync(string gameDomain, CancellationToken ct) => Ok(await _gameQueries.GetAsync(gameDomain, ct));

        [HttpGet("mod/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetModAsync(uint gameId, uint modId, CancellationToken ct) => Ok(await _modQueries.GetAsync(gameId, modId, ct));

        [HttpGet("mod/domain")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GameViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetModAsync(string gameDomain, uint modId, CancellationToken ct) => Ok(await _modQueries.GetAsync(gameDomain, modId, ct));

        [HttpGet("game/all")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IAsyncEnumerable<GameViewModel>), StatusCodes.Status200OK)]
        public IActionResult GetGamesAsync(CancellationToken ct) => Ok(_gameQueries.GetAllAsync(ct));

        [HttpGet("thread/id")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ThreadViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetThreadAsync(uint gameId, uint modId, CancellationToken ct) => Ok(await _threadQueries.GetAsync(gameId, modId, ct));
    }
}