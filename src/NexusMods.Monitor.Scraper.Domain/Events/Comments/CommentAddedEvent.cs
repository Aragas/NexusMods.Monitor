using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Comments
{
    public sealed class CommentAddedEvent : INotification
    {
        public uint Id { get; }

        public CommentAddedEvent(uint id)
        {
            Id = id;
        }
    }
}