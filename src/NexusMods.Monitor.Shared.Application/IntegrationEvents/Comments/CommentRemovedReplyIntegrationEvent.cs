using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed class CommentRemovedReplyIntegrationEvent : Event
    {
        public CommentDTO Comment { get; private set; } = default!;
        public CommentReplyDTO DeletedCommentReply { get; private set; } = default!;

        private CommentRemovedReplyIntegrationEvent() { }
        public CommentRemovedReplyIntegrationEvent(CommentDTO comment, CommentReplyDTO deletedCommentReply) : this()
        {
            Comment = comment;
            DeletedCommentReply = deletedCommentReply;
        }
    }
}