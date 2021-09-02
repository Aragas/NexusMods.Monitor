using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed record IssueAddedReplyEvent(uint Id, uint ReplyId) : INotification;
}