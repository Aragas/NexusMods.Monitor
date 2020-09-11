using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed class IssueChangedPriorityIntegrationEvent : Event
    {
        public IssueDTO Issue { get; private set; } = default!;
        public IssueDTO.IssuePriorityDTO OldIssuePriority { get; private set; } = default!;

        private IssueChangedPriorityIntegrationEvent() { }
        public IssueChangedPriorityIntegrationEvent(IssueDTO issue, IssueDTO.IssuePriorityDTO oldIssuePriority) : this()
        {
            Issue = issue;
            OldIssuePriority = oldIssuePriority;
        }
    }
}