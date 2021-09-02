using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Comments
{
    public sealed record CommentAddedEvent(uint Id) : INotification;
}