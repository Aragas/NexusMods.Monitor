using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed class CommentRemovedIntegrationEvent : Event
    {
        public CommentDTO Comment { get; private set; } = default!;

        private CommentRemovedIntegrationEvent() { }
        public CommentRemovedIntegrationEvent(CommentDTO comment) : this()
        {
            Comment = comment;
        }
    }
}