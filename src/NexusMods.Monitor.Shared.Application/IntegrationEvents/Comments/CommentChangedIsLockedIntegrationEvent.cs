using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed class CommentChangedIsLockedIntegrationEvent : Event
    {
        public CommentDTO Comment { get; private set; } = default!;
        public bool OldIsLocked { get; private set; } = default!;

        private CommentChangedIsLockedIntegrationEvent() { }
        public CommentChangedIsLockedIntegrationEvent(CommentDTO comment, bool oldIsLocked) : this()
        {
            Comment = comment;
            OldIsLocked = oldIsLocked;
        }
    }
}