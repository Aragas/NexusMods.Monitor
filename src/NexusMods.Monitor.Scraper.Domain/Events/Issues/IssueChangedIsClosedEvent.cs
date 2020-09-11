using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed class IssueChangedIsClosedEvent : INotification
    {
        public uint Id { get; }
        public bool OldIsClosed { get; }

        public IssueChangedIsClosedEvent(uint id, bool oldIsClosed)
        {
            Id = id;
            OldIsClosed = oldIsClosed;
        }
    }
}