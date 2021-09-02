namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed record CommentAddedReplyIntegrationEvent(CommentDTO Comment, uint ReplyId) : EventRecord;
}