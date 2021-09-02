using MediatR;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    public sealed record CommentChangeIsStickyCommand(uint Id, bool IsSticky) : IRequest<bool>;
}