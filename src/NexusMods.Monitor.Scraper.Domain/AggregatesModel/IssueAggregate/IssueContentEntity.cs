using NexusMods.Monitor.Shared.Domain.SeedWork;

using NodaTime;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    // Should be a ValueObject
    public sealed class IssueContentEntity : Entity
    {
        public string Author { get; private set; } = default!;
        public string AuthorUrl { get; private set; } = default!;
        public string AvatarUrl { get; private set; } = default!;
        public string Content { get; private set; } = default!;
        public bool IsDeleted { get; private set; } = default!;
        public Instant TimeOfPost { get; private set; } = default!;

        private IssueContentEntity() { }
        public IssueContentEntity(uint id, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant timeOfPost)
        {
            Id = id;
            Author = author;
            AuthorUrl = authorUrl;
            AvatarUrl = avatarUrl;
            Content = content;
            IsDeleted = isDeleted;
            TimeOfPost = timeOfPost;
        }

        public void Remove()
        {
            IsDeleted = true;
        }
    }
}