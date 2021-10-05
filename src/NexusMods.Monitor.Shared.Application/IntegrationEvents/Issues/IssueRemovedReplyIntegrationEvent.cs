using NexusMods.Monitor.Shared.Application.Models;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed record IssueRemovedReplyIntegrationEvent(IssueDTO Issue, IssueReplyDTO Reply) : IssueIntegrationEvent;
}