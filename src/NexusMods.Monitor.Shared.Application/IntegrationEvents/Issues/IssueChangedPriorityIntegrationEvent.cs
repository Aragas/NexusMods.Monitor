using NexusMods.Monitor.Shared.Application.Models;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed record IssueChangedPriorityIntegrationEvent(IssueDTO Issue, IssuePriorityDTO PreviousPriority) : EventRecord;
}