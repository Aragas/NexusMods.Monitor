using MediatR;

namespace NexusMods.Monitor.Scraper.Domain.Events.Comments
{
    public sealed record CommentAddedReplyEvent(uint Id, uint ReplyId) : INotification;
}