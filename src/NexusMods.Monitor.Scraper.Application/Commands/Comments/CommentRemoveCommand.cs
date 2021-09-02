using MediatR;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    public sealed record CommentRemoveCommand(uint Id) : IRequest<bool>;
}