using NexusMods.Monitor.Shared.Domain.SeedWork;

using NodaTime;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    // Should be a ValueObject
    public sealed record IssueContentEntity : Entity
    {
        public string Author { get; private set; }
        public string AuthorUrl { get; private set; }
        public string AvatarUrl { get; private set; }
        public string Content { get; private set; }
        public bool IsDeleted { get; private set; }
        public Instant TimeOfPost { get; private set; }

        private IssueContentEntity() : this(default, default!, default!, default!, default!, default, default) { }
        public IssueContentEntity(uint id, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant timeOfPost) : base(id)
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
        public void Return()
        {
            IsDeleted = true;
        }
    }
}