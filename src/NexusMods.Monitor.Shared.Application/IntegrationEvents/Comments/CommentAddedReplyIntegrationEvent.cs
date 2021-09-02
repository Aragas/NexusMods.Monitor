using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed class CommentAddedReplyIntegrationEvent : Event
    {
        public CommentDTO Comment { get; private set; } = default!;
        public uint ReplyId { get; private set; } = default!;

        private CommentAddedReplyIntegrationEvent() { }
        public CommentAddedReplyIntegrationEvent(CommentDTO comment, uint replyId) : this()
        {
            Comment = comment;
            ReplyId = replyId;
        }
    }
}