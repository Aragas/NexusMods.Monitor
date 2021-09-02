using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed record IssueChangedPriorityIntegrationEvent(IssueDTO Issue, IssuePriorityDTO OldIssuePriority) : EventRecord;
}