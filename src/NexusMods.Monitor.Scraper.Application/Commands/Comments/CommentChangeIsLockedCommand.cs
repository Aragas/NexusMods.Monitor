using MediatR;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    public sealed record CommentChangeIsLockedCommand(uint Id, bool IsLocked) : IRequest<bool>;
}