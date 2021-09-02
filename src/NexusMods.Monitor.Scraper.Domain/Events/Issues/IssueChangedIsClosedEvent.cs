using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed record IssueChangedIsClosedEvent(uint Id, bool OldIsClosed) : INotification;
}