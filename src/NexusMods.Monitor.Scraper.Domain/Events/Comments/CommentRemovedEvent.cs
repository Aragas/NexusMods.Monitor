using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Comments
{
    public sealed class CommentRemovedEvent : INotification
    {
        public uint Id { get; }

        public CommentRemovedEvent(uint id)
        {
            Id = id;
        }
    }
}