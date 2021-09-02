using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Comments
{
    public sealed record CommentRemovedReplyEvent(uint Id, uint ReplyId) : INotification;
}