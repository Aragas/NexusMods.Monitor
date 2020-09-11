using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed class CommentAddedIntegrationEvent : Event
    {
        public CommentDTO Comment { get; private set; } = default!;

        public CommentAddedIntegrationEvent(CommentDTO comment)
        {
            Comment = comment;
        }
    }
}