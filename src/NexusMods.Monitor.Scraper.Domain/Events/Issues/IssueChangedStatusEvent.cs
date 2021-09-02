using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed record IssueChangedStatusEvent(uint Id, int OldIssueStatusId) : INotification;
}