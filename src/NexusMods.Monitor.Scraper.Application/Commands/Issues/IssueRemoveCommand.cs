using MediatR;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    public sealed record IssueRemoveCommand(uint Id) : IRequest<bool>;
}