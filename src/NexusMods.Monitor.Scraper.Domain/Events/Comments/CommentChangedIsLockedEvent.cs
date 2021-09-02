using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Comments
{
    public sealed record CommentChangedIsLockedEvent(uint Id, bool OldIsLocked) : INotification;
}