using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.CommandHandlers.Issues
{
    public sealed class IssueAddCommandHandler : IRequestHandler<IssueAddCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly IIssueRepository _issueRepository;

        public IssueAddCommandHandler(ILogger<IssueAddCommandHandler> logger, IIssueRepository issueRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
        }

        public async Task<bool> Handle(IssueAddCommand message, CancellationToken ct)
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

            var issueEntity = Mapper.Map(message, await _issueRepository.GetStatusAsync(message.StatusId), await _issueRepository.GetPriorityAsync(message.PriorityId));

            _issueRepository.Add(issueEntity);

            return await _issueRepository.UnitOfWork.SaveEntitiesAsync(ct);
        }
    }
}