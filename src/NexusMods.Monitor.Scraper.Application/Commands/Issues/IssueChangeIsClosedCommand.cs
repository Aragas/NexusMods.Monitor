using MediatR;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    public sealed record IssueChangeIsClosedCommand(uint Id, bool IsClosed) : IRequest<bool>;
}