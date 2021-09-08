using NodaTime;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public sealed record NexusModsCommentReplyViewModel(uint Id, uint OwnerId, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant Post);
}