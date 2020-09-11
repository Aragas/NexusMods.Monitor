using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed class IssueChangedStatusEvent : INotification
    {
        public uint Id { get; }
        public int OldIssueStatusId { get; }

        public IssueChangedStatusEvent(uint id, int oldIssueStatusId)
        {
            Id = id;
            OldIssueStatusId = oldIssueStatusId;
        }
    }
}