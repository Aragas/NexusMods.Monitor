using NexusMods.Monitor.Scraper.Domain.Events.Comments;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using NodaTime;

using System.Diagnostics.CodeAnalysis;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate
{
    [SuppressMessage("ReSharper", "RedundantBoolCompare", Justification = "Consistent look")]
    public sealed record CommentReplyEntity : DefaultEntity
    {
        public uint OwnerId { get; private set; }
        public string Url { get; private set; }
        public string Author { get; private set; }
        public string AuthorUrl { get; private set; }
        public string AvatarUrl { get; private set; }
        public string Content { get; private set; }
        public bool IsDeleted { get; private set; }
        public Instant TimeOfPost { get; private set; }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private CommentReplyEntity() : this(default, default, default!, default!, default!, default!, default!, default, default) { }
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
            if (IsDeleted != true)
            {
                AddDomainEvent(new CommentRemovedReplyEvent(OwnerId, Id));
                IsDeleted = true;
            }
        }
        public void Return()
        {
            if (IsDeleted != false)
            {
                AddDomainEvent(new CommentAddedReplyEvent(OwnerId, Id));
                IsDeleted = false;
            }
        }
    }
}