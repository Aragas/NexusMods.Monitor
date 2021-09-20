using NexusMods.Monitor.Shared.Application.Models;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed record CommentAddedReplyIntegrationEvent(CommentDTO Comment, CommentReplyDTO Reply) : EventRecord;
}