using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Comments
{
    public sealed class CommentAddedReplyEvent : INotification
    {
        public uint Id { get; }
        public uint ReplyId { get; }

        public CommentAddedReplyEvent(uint id, uint replyId)
        {
            Id = id;
            ReplyId = replyId;
        }
    }
}