using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed class IssueRemovedReplyEvent : INotification
    {
        public uint Id { get; }
        public uint ReplyId { get; }

        public IssueRemovedReplyEvent(uint id, uint replyId)
        {
            Id = id;
            ReplyId = replyId;
        }
    }
}