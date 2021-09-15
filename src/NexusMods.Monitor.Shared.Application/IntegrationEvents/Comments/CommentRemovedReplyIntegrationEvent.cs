using NexusMods.Monitor.Shared.Application.Models;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed record CommentRemovedReplyIntegrationEvent(CommentDTO Comment, CommentReplyDTO DeletedCommentReply) : EventRecord;
}