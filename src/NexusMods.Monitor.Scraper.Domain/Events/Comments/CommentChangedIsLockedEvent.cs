using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Comments
{
    public sealed class CommentChangedIsLockedEvent : INotification
    {
        public uint Id { get; }
        public bool OldIsLocked { get; }

        public CommentChangedIsLockedEvent(uint id, bool oldIsLocked)
        {
            Id = id;
            OldIsLocked = oldIsLocked;
        }
    }
}