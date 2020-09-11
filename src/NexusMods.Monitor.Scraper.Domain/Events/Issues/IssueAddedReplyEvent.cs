using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed class IssueAddedReplyEvent : INotification
    {
        public uint Id { get; }
        public uint ReplyId { get; }

        public IssueAddedReplyEvent(uint id, uint replyId)
        {
            Id = id;
            ReplyId = replyId;
        }
    }
}