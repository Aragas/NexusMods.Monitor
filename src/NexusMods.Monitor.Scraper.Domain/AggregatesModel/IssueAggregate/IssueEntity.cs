using NexusMods.Monitor.Scraper.Domain.Events.Issues;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using NodaTime;

using System.Collections.Generic;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    public sealed class IssueEntity : Entity, IAggregateRoot
    {
        public uint NexusModsModId { get; private set; } = default!;
        public uint NexusModsGameId { get; private set; } = default!;
        public string Title { get; private set; } = default!;
        public string Url { get; private set; } = default!;
        public string ModVersion { get; private set; } = default!;
        public IssueStatusEnumeration Status { get; private set; } = default!;
        public IssuePriorityEnumeration Priority { get; private set; } = default!;
        public bool IsPrivate { get; private set; } = default!;
        public bool IsClosed { get; private set; } = default!;
        public bool IsDeleted { get; private set; } = default!;
        public Instant TimeOfLastPost { get; private set; } = default!;
        public IssueContentEntity? Content { get; private set; } = default!;

        private readonly List<IssueReplyEntity> _replies = default!;
        public IEnumerable<IssueReplyEntity> Replies => _replies;

        private IssueEntity()
        {
            _replies = new List<IssueReplyEntity>();
        }
        public IssueEntity(uint id, uint nexusModsGameId, uint nexusModsModId, string title, string url, string modVersion, IssueStatusEnumeration status, IssuePriorityEnumeration priority, bool isPrivate, bool isClosed, bool isDeleted, Instant timeOfLastPost) : this()
        {
            Id = id;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            Title = title;
            Url = url;
            ModVersion = modVersion;
            Status = status;
            Priority = priority;
            IsPrivate = isPrivate;
            IsClosed = isClosed;
            IsDeleted = isDeleted;
            TimeOfLastPost = timeOfLastPost;

            AddDomainEvent(new IssueAddedEvent(Id));
        }

        public void Remove()
        {
            IsDeleted = true;
            Content?.Remove();
            AddDomainEvent(new IssueRemovedEvent(Id));
        }

        public void SetContent(string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant timeOfPost)
        {
            Content = new IssueContentEntity(Id, author, authorUrl, avatarUrl, content, isDeleted, timeOfPost);
        }

        public void SetIsClosed(bool isClosed)
        {
            if (IsClosed != isClosed)
            {
                AddDomainEvent(new IssueChangedIsClosedEvent(Id, IsClosed));
                IsClosed = isClosed;
            }
        }

        public void SetIsPrivate(bool iPrivate)
        {
            if (IsPrivate != iPrivate)
            {
                AddDomainEvent(new IssueChangedIsPrivateEvent(Id, IsPrivate));
                IsPrivate = iPrivate;
            }
        }

        public void SetPriority(IssuePriorityEnumeration priority)
        {
            if (Priority.Id != priority.Id)
            {
                AddDomainEvent(new IssueChangedPriorityEvent(Id, Priority.Id));
                Priority = priority;
            }
        }

        public void SetStatus(IssueStatusEnumeration status)
        {
            if (Status.Id != status.Id)
            {
                AddDomainEvent(new IssueChangedStatusEvent(Id, Status.Id));
                Status = status;
            }
        }

        public void RemoveReply(uint id)
        {
            var issueReplyEntity = Replies.SingleOrDefault(o => o.Id == id);
            if (!(issueReplyEntity is null))
            {
                issueReplyEntity.Remove();
                AddDomainEvent(new IssueRemovedReplyEvent(Id, id));
            }
        }

        public IssueReplyEntity AddReplyEntity(uint id, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant timeOfPost)
        {
            var existingCommentReplyEntity = Replies.SingleOrDefault(o => o.Id == id);
            if (existingCommentReplyEntity is null)
            {
                var issueReplyEntity = new IssueReplyEntity(id, Id, author, authorUrl, avatarUrl, content, isDeleted, timeOfPost);
                _replies.Add(issueReplyEntity);
                AddDomainEvent(new IssueAddedReplyEvent(Id, id));
                return issueReplyEntity;
            }
            else
            {
                AddDomainEvent(new IssueAddedReplyEvent(Id, id));
                return existingCommentReplyEntity;
            }
        }
    }
}