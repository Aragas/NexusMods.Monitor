using NexusMods.Monitor.Scraper.Domain.Events.Issues;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using NodaTime;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    public sealed record IssueReplyEntity(uint Id) : Entity(Id)
    {
        public uint OwnerId { get; private set; } = default!;
        public string Author { get; private set; } = default!;
        public string AuthorUrl { get; private set; } = default!;
        public string AvatarUrl { get; private set; } = default!;
        public string Content { get; private set; } = default!;
        public bool IsDeleted { get; private set; } = default!;
        public Instant TimeOfPost { get; private set; } = default!;

        private IssueReplyEntity() : this(default, default, default!, default!, default!, default!, default, default) { }
        public IssueReplyEntity(uint id, uint ownerId, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant timeOfPost) : this(id)
        {
            OwnerId = ownerId;
            Author = author;
            AuthorUrl = authorUrl;
            AvatarUrl = avatarUrl;
            Content = content;
            IsDeleted = isDeleted;
            TimeOfPost = timeOfPost;
            AddDomainEvent(new IssueReplyAddedEvent(OwnerId, Id));
        }

        public void Remove()
        {
            if (IsDeleted != true)
            {
                AddDomainEvent(new IssueReplyRemovedEvent(OwnerId, Id));
                IsDeleted = true;
            }
        }
        public void Return()
        {
            if (IsDeleted != false)
            {
                AddDomainEvent(new IssueReplyAddedEvent(OwnerId, Id));
                IsDeleted = false;
            }
        }
    }
}