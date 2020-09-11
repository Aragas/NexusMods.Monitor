using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed class IssueAddedIntegrationEvent : Event
    {
        public IssueDTO Issue { get; private set; } = default!;

        private IssueAddedIntegrationEvent() { }
        public IssueAddedIntegrationEvent(IssueDTO issue) : this()
        {
            Issue = issue;
        }
    }
}