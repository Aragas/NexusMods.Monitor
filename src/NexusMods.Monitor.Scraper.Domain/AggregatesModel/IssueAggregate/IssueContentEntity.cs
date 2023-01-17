using NexusMods.Monitor.Scraper.Domain.Events.Issues;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using NodaTime;

using System.Diagnostics.CodeAnalysis;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    [SuppressMessage("ReSharper", "RedundantBoolCompare", Justification = "Consistent look")]
    public sealed record IssueContentEntity(uint Id) : DefaultEntity(Id)
    {
        public string Author { get; private set; } = default!;
        public string AuthorUrl { get; private set; } = default!;
        public string AvatarUrl { get; private set; } = default!;
        public string Content { get; private set; } = default!;
        public bool IsDeleted { get; private set; } = default!;
        public Instant TimeOfPost { get; private set; } = default!;

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private IssueContentEntity() : this(default, default!, default!, default!, default!, default, default) { }
        public IssueContentEntity(uint id, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant timeOfPost) : this(id)
        {
            Author = author;
            AuthorUrl = authorUrl;
            AvatarUrl = avatarUrl;
            Content = content;
            IsDeleted = isDeleted;
            TimeOfPost = timeOfPost;

            AddDomainEvent(new IssueContentChangedEvent(Id));
        }

        public void Remove()
        {
            if (IsDeleted != true)
            {
                AddDomainEvent(new IssueContentChangedEvent(Id));
                IsDeleted = true;
            }
        }
        public void Return()
        {
            if (IsDeleted != false)
            {
                AddDomainEvent(new IssueContentChangedEvent(Id));
                IsDeleted = false;
            }
        }
    }
}