using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed class IssueChangedIsClosedIntegrationEvent : Event
    {
        public IssueDTO Issue { get; private set; } = default!;
        public bool OldIsClosed { get; private set; } = default!;

        private IssueChangedIsClosedIntegrationEvent() { }
        public IssueChangedIsClosedIntegrationEvent(IssueDTO issue, bool oldIsClosed) : this()
        {
            Issue = issue;
            OldIsClosed = oldIsClosed;
        }
    }
}