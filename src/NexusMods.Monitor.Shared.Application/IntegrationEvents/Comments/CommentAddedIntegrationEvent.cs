namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed record CommentAddedIntegrationEvent(CommentDTO Comment) : EventRecord;
}