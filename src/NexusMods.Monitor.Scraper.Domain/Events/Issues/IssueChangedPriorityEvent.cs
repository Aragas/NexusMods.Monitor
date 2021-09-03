using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed record IssueChangedPriorityEvent(uint Id, uint OldIssuePriorityId) : INotification;
}