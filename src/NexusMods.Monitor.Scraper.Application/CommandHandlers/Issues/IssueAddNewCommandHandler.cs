using AutoMapper;

using Enbiso.NLib.EventBus;

using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Application;
using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public sealed class IssueAddNewCommandHandler : IRequestHandler<IssueAddNewCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public IssueAddNewCommandHandler(ILogger<IssueAddNewCommandHandler> logger, IIssueRepository issueRepository, IMapper mapper, IEventPublisher eventPublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public async Task<bool> Handle(IssueAddNewCommand message, CancellationToken ct)
        {
            var existingIssueEntity = await _issueRepository.GetAsync(message.Id);
            if (existingIssueEntity is { })
            {
                if (existingIssueEntity.IsDeleted)
                {
                    existingIssueEntity.Return();
                    return await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct);
                }

                _logger.LogError("Issue with Id {Id} already exist, is not deleted.", message.Id);
                return false;
            }

            var issueEntity = new IssueEntity(
                message.Id,
                message.NexusModsGameId,
                message.NexusModsModId,
                message.GameName,
                message.ModName,
                message.Title,
                message.Url,
                message.ModVersion,
                await _issueRepository.GetStatusAsync(message.Status.Id),
                await _issueRepository.GetPriorityAsync(message.Priority.Id),
                message.IsPrivate,
                message.IsClosed,
                message.IsDeleted,
                message.TimeOfLastPost);

            if (message.Content is not null)
            {
                issueEntity.SetContent(
                    message.Content.Author,
                    message.Content.AuthorUrl,
                    message.Content.AvatarUrl,
                    message.Content.Content,
                    false,
                    message.Content.TimeOfPost);
            }

            foreach (var issueReply in message.Replies)
            {
                issueEntity.AddReplyEntity(
                    issueReply.Id,
                    issueReply.Author,
                    issueReply.AuthorUrl,
                    issueReply.AvatarUrl,
                    issueReply.Content,
                    false,
                    issueReply.TimeOfPost);
            }

            _issueRepository.Add(issueEntity);

            var issueDTO = _mapper.Map<IssueEntity, IssueDTO>(issueEntity);

            if (await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                await _eventPublisher.Publish(new IssueAddedIntegrationEvent(issueDTO), "issue_events", ct);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}