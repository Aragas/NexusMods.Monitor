using MediatR;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    public sealed record IssueChangeIsPrivateCommand(uint Id, bool IsPrivate) : IRequest<bool>;
}