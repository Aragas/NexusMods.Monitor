using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed record IssueRemovedReplyEvent(uint Id, uint ReplyId) : INotification;
}