using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Subscriptions.Application.Commands;
using NexusMods.Monitor.Subscriptions.Application.Queries.Subscriptions;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public sealed class SubscriptionsController : ControllerBase
    {
        [SuppressMessage("CodeQuality", "IDE0052", Justification = "Reserved for future use")]
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly ISubscriptionQueries _subscriptionQueries;

        public SubscriptionsController(ILogger<SubscriptionsController> logger, IMediator mediator, ISubscriptionQueries subscriptionQueries)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
        }

        [HttpPut("subscribe")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubscribeAsync([FromBody] SubscriptionAddCommand command) => await _mediator.Send(command) ? Ok() : StatusCode((int) HttpStatusCode.BadRequest);

        [HttpPut("subscribe2")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Subscribe2Async([FromBody] SubscriptionAdd2Command command) => await _mediator.Send(command) ? Ok() : StatusCode((int) HttpStatusCode.BadRequest);

        [HttpPut("unsubscribe")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UnsubscribeAsync([FromBody] SubscriptionRemoveCommand command) => await _mediator.Send(command) ? Ok() : StatusCode((int) HttpStatusCode.BadRequest);

        [HttpPut("unsubscribe2")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Unsubscribe2Async([FromBody] SubscriptionRemove2Command command) => await _mediator.Send(command) ? Ok() : StatusCode((int) HttpStatusCode.BadRequest);

        [HttpGet("all")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK, Type = typeof(IAsyncEnumerable<SubscriptionViewModel>))]
        public IActionResult GetAllAsync() => Ok(_subscriptionQueries.GetSubscriptionsAsync());
    }
}