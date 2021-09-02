using NexusMods.Monitor.Scraper.Domain.Events.Comments;
using NexusMods.Monitor.Shared.Domain;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using NodaTime;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate
{
    public sealed record CommentReplyEntity : Entity
    {
        public uint OwnerId { get; private set; }
        public string Url { get; private set; }
        public string Author { get; private set; }
        public string AuthorUrl { get; private set; }
        public string AvatarUrl { get; private set; }
        public string Content { get; private set; }
        public bool IsDeleted { get; private set; }
        public Instant TimeOfPost { get; private set; }

        private CommentReplyEntity() : this(RecordUtils.Default<CommentReplyEntity>()) { }
        public CommentReplyEntity(uint id, uint ownerId, string url, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant timeOfPost) : base(id)
        {
            Id = id;
            OwnerId = ownerId;
            Url = url;
            Author = author;
            AuthorUrl = authorUrl;
            AvatarUrl = avatarUrl;
            Content = content;
            IsDeleted = isDeleted;
            TimeOfPost = timeOfPost;

            AddDomainEvent(new CommentAddedReplyEvent(OwnerId, Id));
        }

        public void Remove()
        {
            IsDeleted = true;
            AddDomainEvent(new CommentRemovedReplyEvent(OwnerId, Id));
        }
        public void Return()
        {
            IsDeleted = false;
            AddDomainEvent(new CommentAddedReplyEvent(OwnerId, Id));
        }
    }
}