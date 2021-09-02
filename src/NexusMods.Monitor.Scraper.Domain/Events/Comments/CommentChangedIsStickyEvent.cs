using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Comments
{
    public sealed record CommentChangedIsStickyEvent(uint Id, bool OldIsSticky) : INotification;
}