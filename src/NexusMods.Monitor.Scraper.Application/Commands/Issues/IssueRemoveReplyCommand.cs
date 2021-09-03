using MediatR;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    public sealed record IssueRemoveReplyCommand(uint Id, uint ReplyId) : IRequest<bool>;
}