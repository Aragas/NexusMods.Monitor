using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed class CommentChangedIsStickyIntegrationEvent : Event
    {
        public CommentDTO Comment { get; private set; } = default!;
        public bool OldIsSticky { get; private set; } = default!;

        private CommentChangedIsStickyIntegrationEvent() { }
        public CommentChangedIsStickyIntegrationEvent(CommentDTO comment, bool oldIsSticky) : this()
        {
            Comment = comment;
            OldIsSticky = oldIsSticky;
        }
    }
}