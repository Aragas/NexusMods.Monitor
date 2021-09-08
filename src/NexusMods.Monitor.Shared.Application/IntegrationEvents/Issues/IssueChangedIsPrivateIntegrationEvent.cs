namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed record IssueChangedIsPrivateIntegrationEvent(IssueDTO Issue, bool OldIsPrivate) : EventRecord;
}