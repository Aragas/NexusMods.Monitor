using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed class IssueAddedEvent : INotification
    {
        public uint Id { get; }

        public IssueAddedEvent(uint id)
        {
            Id = id;
        }
    }
}