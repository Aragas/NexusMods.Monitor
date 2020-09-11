using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Comments
{
    public sealed class CommentRemovedReplyEvent : INotification
    {
        public uint Id { get; }
        public uint ReplyId { get; }

        public CommentRemovedReplyEvent(uint id, uint replyId)
        {
            Id = id;
            ReplyId = replyId;
        }
    }
}