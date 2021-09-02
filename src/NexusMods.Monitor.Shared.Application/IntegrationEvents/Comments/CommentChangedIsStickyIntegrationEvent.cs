namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed record CommentChangedIsStickyIntegrationEvent(CommentDTO Comment, bool OldIsSticky) : EventRecord;
}