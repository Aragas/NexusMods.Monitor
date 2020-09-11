using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Subscriptions.Application.Commands;
using NexusMods.Monitor.Subscriptions.Application.Queries;

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly ISubscriptionQueries _subscriptionQueries;

        public SubscriptionsController(ILogger<SubscriptionsController> logger, IMediator mediator, ISubscriptionQueries subscriptionQueries)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
        }

        [Route("subscribe")]
        [HttpPut]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SubscribeAsync([FromBody] SubscriptionAddCommand command) => await _mediator.Send(command) ?  (IActionResult) Ok() : (IActionResult) BadRequest();

        [Route("unsubscribe")]
        [HttpPut]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UnsubscribeAsync([FromBody] SubscriptionRemoveCommand command) => await _mediator.Send(command) ?  (IActionResult) Ok() : (IActionResult) BadRequest();

        [Route("all")]
        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllAsync() => Ok(await _subscriptionQueries.GetSubscriptionsAsync().ToListAsync());
    }
}
