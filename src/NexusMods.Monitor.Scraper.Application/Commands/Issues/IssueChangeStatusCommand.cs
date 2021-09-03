using MediatR;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    public sealed record IssueChangeStatusCommand(uint Id, uint StatusId) : IRequest<bool>;
}