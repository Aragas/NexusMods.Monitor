using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Comments
{
    public sealed record CommentRemovedEvent(uint Id) : INotification;
}