using MediatR;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    public sealed record IssueChangePriorityCommand(uint Id, uint PriorityId) : IRequest<bool>;
}