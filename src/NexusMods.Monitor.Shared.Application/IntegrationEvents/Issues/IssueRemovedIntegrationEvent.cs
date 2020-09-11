using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed class IssueRemovedIntegrationEvent : Event
    {
        public IssueDTO Issue { get; private set; } = default!;

        private IssueRemovedIntegrationEvent() { }
        public IssueRemovedIntegrationEvent(IssueDTO issue) : this()
        {
            Issue = issue;
        }
    }
}