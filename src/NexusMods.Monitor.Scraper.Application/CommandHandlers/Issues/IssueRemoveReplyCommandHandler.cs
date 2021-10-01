using Enbiso.NLib.EventBus;

using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public sealed class IssueRemoveReplyCommandHandler : IRequestHandler<IssueRemoveReplyCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;
        private readonly IEventPublisher _eventPublisher;

        public IssueRemoveReplyCommandHandler(ILogger<IssueRemoveReplyCommandHandler> logger, IIssueRepository issueRepository, IEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(IssueRemoveReplyCommand message, CancellationToken ct)
        {
            var issueEntity = await _issueRepository.GetAsync(message.Id);
            if (issueEntity is null)
            {
                _logger.LogError("Issue with Id {Id} was not found.", message.Id);
                return false;
            }

            if (issueEntity.Replies.All(r => r.Id != message.Id))
            {
                _logger.LogError("Issue with Id {Id} doesn't have the reply! IssueReply Id {ReplyId}", message.Id, message.ReplyId);
                return false;
            }

            var issueReplyDTO = Mapper.Map(issueEntity.Replies.First(x => x.Id == message.ReplyId));

            issueEntity.RemoveReply(message.ReplyId);
            _issueRepository.Update(issueEntity);

            var issueDTO = Mapper.Map(issueEntity);

            if (await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                await _eventPublisher.Publish(new IssueRemovedReplyIntegrationEvent(issueDTO, issueReplyDTO), "issue_events", ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}