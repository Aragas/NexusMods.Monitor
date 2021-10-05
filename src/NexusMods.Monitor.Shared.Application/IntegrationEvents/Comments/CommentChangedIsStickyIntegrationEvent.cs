using NexusMods.Monitor.Shared.Application.Models;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed record CommentChangedIsStickyIntegrationEvent(CommentDTO Comment, bool PreviousIsSticky) : CommentIntegrationEvent;
}