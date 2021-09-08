﻿namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed record IssueChangedStatusIntegrationEvent(IssueDTO Issue, IssueStatusDTO OldIssueStatus) : EventRecord;
}