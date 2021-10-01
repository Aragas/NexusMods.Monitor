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
    public sealed class IssueAddNewReplyCommandHandler : IRequestHandler<IssueAddNewReplyCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;
        private readonly IEventPublisher _eventPublisher;

        public IssueAddNewReplyCommandHandler(ILogger<IssueAddNewReplyCommandHandler> logger, IIssueRepository issueRepository, IEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(IssueAddNewReplyCommand message, CancellationToken ct)
        {
            var issueEntity = await _issueRepository.GetAsync(message.Id);
            if (issueEntity is null)
            {
                _logger.LogError("Issue with Id {Id} was not found! IssueReply Id {ReplyId}", message.Id, message.ReplyIdId);
                return false;
            }

            if (issueEntity.Replies.Any(r => r.Id == message.ReplyIdId))
            {
                _logger.LogError("Issue with Id {Id} has already the reply! IssueReply Id {ReplyId}", message.Id, message.ReplyIdId);
                return false;
            }

            var issueReplyEntity = issueEntity.AddReplyEntity(message.ReplyIdId, message.Author, message.AuthorUrl, message.AvatarUrl, message.Content, false, message.TimeOfPost);

            _issueRepository.Update(issueEntity);

            var issueDTO = Mapper.Map(issueEntity);
            var issueReplyDTO = Mapper.Map(issueReplyEntity);

            if (await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                await _eventPublisher.Publish(new IssueAddedReplyIntegrationEvent(issueDTO, issueReplyDTO), "issue_events", ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}