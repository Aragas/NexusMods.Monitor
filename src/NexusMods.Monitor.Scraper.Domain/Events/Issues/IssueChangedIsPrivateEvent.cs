using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed class IssueChangedIsPrivateEvent : INotification
    {
        public uint Id { get; }
        public bool OldIsPrivate { get; }

        public IssueChangedIsPrivateEvent(uint id, bool oldIsPrivate)
        {
            Id = id;
            OldIsPrivate = oldIsPrivate;
        }
    }
}