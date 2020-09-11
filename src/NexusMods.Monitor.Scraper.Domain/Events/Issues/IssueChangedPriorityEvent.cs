using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed class IssueChangedPriorityEvent : INotification
    {
        public uint Id { get; }
        public int OldIssuePriorityId { get; }

        public IssueChangedPriorityEvent(uint id, int oldIssuePriorityId)
        {
            Id = id;
            OldIssuePriorityId = oldIssuePriorityId;
        }
    }
}