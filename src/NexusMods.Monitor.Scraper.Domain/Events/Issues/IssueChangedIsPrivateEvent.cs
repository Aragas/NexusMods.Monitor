using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed record IssueChangedIsPrivateEvent(uint Id, bool OldIsPrivate) : INotification;
}