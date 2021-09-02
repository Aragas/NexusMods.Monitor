namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed record IssueRemovedIntegrationEvent(IssueDTO Issue) : EventRecord;
}