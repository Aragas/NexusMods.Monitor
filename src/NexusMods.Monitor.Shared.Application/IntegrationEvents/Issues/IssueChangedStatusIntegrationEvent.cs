using NexusMods.Monitor.Shared.Application.Models;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed record IssueChangedStatusIntegrationEvent(IssueDTO Issue, IssueStatusDTO PreviousStatus) : IssueIntegrationEvent;
}