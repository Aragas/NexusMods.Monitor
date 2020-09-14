using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed class IssueChangedPriorityIntegrationEvent : Event
    {
        public IssueDTO Issue { get; private set; } = default!;
        public IssuePriorityDTO OldIssuePriority { get; private set; } = default!;

        private IssueChangedPriorityIntegrationEvent() { }
        public IssueChangedPriorityIntegrationEvent(IssueDTO issue, IssuePriorityDTO oldIssuePriority) : this()
        {
            Issue = issue;
            OldIssuePriority = oldIssuePriority;
        }
    }
}