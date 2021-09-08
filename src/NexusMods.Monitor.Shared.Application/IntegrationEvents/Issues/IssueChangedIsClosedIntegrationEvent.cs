namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed record IssueChangedIsClosedIntegrationEvent(IssueDTO Issue, bool OldIsClosed) : EventRecord;
}