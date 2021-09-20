using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed record IssueReplyRemovedEvent(uint Id, uint ReplyId) : INotification;
}