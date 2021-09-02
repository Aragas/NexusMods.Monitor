using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Application.Queries.Comments
{
    public sealed record CommentViewModel(uint Id, uint NexusModsGameId, uint NexusModsModId, bool IsLocked, bool IsSticky, IReadOnlyList<CommentReplyViewModel> Replies);
}