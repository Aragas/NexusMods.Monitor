using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Comments
{
    public sealed class CommentChangedIsStickyEvent : INotification
    {
        public uint Id { get; }
        public bool OldIsSticky { get; }

        public CommentChangedIsStickyEvent(uint id, bool oldIsSticky)
        {
            Id = id;
            OldIsSticky = oldIsSticky;
        }
    }
}