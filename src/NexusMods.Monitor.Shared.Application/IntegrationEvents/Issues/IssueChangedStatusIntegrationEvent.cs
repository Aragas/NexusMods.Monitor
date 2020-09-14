using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed class IssueChangedStatusIntegrationEvent : Event
    {
        public IssueDTO Issue { get; private set; } = default!;
        public IssueStatusDTO OldIssueStatus { get; private set; } = default!;

        private IssueChangedStatusIntegrationEvent() { }
        public IssueChangedStatusIntegrationEvent(IssueDTO issue, IssueStatusDTO oldIssueStatus) : this()
        {
            Issue = issue;
            OldIssueStatus = oldIssueStatus;
        }
    }
}