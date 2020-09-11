using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Issues
{
    public sealed class IssueRemovedEvent : INotification
    {
        public uint Id { get; }

        public IssueRemovedEvent(uint id)
        {
            Id = id;
        }
    }
}