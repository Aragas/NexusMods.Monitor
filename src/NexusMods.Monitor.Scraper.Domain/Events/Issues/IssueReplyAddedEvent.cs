using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed record IssueReplyAddedEvent(uint Id, uint ReplyId) : INotification;
}