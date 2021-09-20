using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed record IssueContentChangedEvent(uint Id) : INotification;
}