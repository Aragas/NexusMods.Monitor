using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed record IssueChangedStatusEvent(uint Id, uint OldIssueStatusId) : INotification;
}