using MediatR;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    public sealed record CommentRemoveReplyCommand(uint Id, uint ReplyId) : IRequest<bool>;
}