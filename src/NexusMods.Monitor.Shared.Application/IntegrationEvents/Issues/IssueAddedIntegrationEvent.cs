using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed record IssueAddedIntegrationEvent(IssueDTO Issue) : EventRecord;
}