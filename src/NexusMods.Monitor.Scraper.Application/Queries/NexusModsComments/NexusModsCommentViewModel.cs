using NodaTime;

using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public record NexusModsCommentViewModel(uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, bool IsSticky, bool IsLocked, Instant Post, IReadOnlyList<NexusModsCommentReplyViewModel> Replies);
}