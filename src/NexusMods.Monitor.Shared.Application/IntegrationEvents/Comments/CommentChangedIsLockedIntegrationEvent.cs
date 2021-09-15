using NexusMods.Monitor.Shared.Application.Models;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public sealed record CommentChangedIsLockedIntegrationEvent(CommentDTO Comment, bool OldIsLocked) : EventRecord;
}