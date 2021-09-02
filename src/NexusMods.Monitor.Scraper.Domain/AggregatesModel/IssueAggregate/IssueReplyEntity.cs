using NexusMods.Monitor.Shared.Domain;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using NodaTime;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    public sealed record IssueReplyEntity : Entity
    {
        public uint OwnerId { get; private set; }
        public string Author { get; private set; }
        public string AuthorUrl { get; private set; }
        public string AvatarUrl { get; private set; }
        public string Content { get; private set; }
        public bool IsDeleted { get; private set; }
        public Instant TimeOfPost { get; private set; }

        private IssueReplyEntity() : this(RecordUtils.Default<IssueReplyEntity>()) { }
        public IssueReplyEntity(uint id, uint ownerId, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant timeOfPost) : base(id)
        {
            Id = id;
            OwnerId = ownerId;
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