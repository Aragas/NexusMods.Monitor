﻿namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed record IssueRemovedReplyIntegrationEvent(IssueDTO Issue, IssueReplyDTO IssueReply) : EventRecord;
}