using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed class IssueChangedIsPrivateIntegrationEvent : Event
    {
        public IssueDTO Issue { get; private set; } = default!;
        public bool OldIsPrivate { get; private set; } = default!;

        private IssueChangedIsPrivateIntegrationEvent() { }
        public IssueChangedIsPrivateIntegrationEvent(IssueDTO issue, bool oldIsPrivate) : this()
        {
            Issue = issue;
            OldIsPrivate = oldIsPrivate;
        }
    }
}