using NexusMods.Monitor.Scraper.Domain.Events.Issues;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using NodaTime;

using System.Collections.Generic;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    public sealed record IssueEntity(uint Id) : Entity(Id), IAggregateRoot
    {
        public uint NexusModsModId { get; private init; } = default!;
        public uint NexusModsGameId { get; private init; } = default!;
        public string GameName { get; private init; } = default!;
        public string ModName { get; private init; } = default!;
        public string Title { get; private init; } = default!;
        public string Url { get; private init; } = default!;
        public string ModVersion { get; private init; } = default!;
        public IssueStatusEnumeration Status { get; private set; } = default!;
        public IssuePriorityEnumeration Priority { get; private set; } = default!;
        public bool IsPrivate { get; private set; } = default!;
        public bool IsClosed { get; private set; } = default!;
        public bool IsDeleted { get; private set; } = default!;
        public Instant TimeOfLastPost { get; private init; } = default!;
        public IssueContentEntity? Content { get; private set; } = default!;

        private readonly List<IssueReplyEntity> _replies = default!;
        public IEnumerable<IssueReplyEntity> Replies => _replies;

        private IssueEntity() : this(default, default, default, default!, default!, default!, default!, default!, default!, default!, default, default, default, default) { }
        public IssueEntity(uint id, uint nexusModsGameId, uint nexusModsModId, string gameName, string modName, string title, string url, string modVersion, IssueStatusEnumeration status, IssuePriorityEnumeration priority, bool isPrivate, bool isClosed, bool isDeleted, Instant timeOfLastPost) : this(id)
        {
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            GameName = gameName;
            ModName = modName;
            Title = title;
            Url = url;
            ModVersion = modVersion;
            Status = status;
            Priority = priority;
            IsPrivate = isPrivate;
            IsClosed = isClosed;
            IsDeleted = isDeleted;
            TimeOfLastPost = timeOfLastPost;
            _replies = new List<IssueReplyEntity>();

            AddDomainEvent(new IssueAddedEvent(Id));
        }

        public void Remove()
        {
            if (IsDeleted != true)
            {
                AddDomainEvent(new IssueRemovedEvent(Id));
                IsDeleted = true;
                foreach (var reply in Replies)
                {
                    reply.Remove();
                }
                Content?.Remove();
            }
        }
        public void Return()
        {
            if (IsDeleted != false)
            {
                AddDomainEvent(new IssueAddedEvent(Id));
                IsDeleted = false;
                foreach (var reply in Replies)
                {
                    reply.Return();
                }
                Content?.Return();
            }
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
            if (issueReplyEntity is not null)
            {
                issueReplyEntity.Remove();
            }
        }

        public IssueReplyEntity AddReplyEntity(uint id, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant timeOfPost)
        {
            var existingCommentReplyEntity = Replies.SingleOrDefault(o => o.Id == id);
            if (existingCommentReplyEntity is null)
            {
                var issueReplyEntity = new IssueReplyEntity(id, Id, author, authorUrl, avatarUrl, content, isDeleted, timeOfPost);
                _replies.Add(issueReplyEntity);
                return issueReplyEntity;
            }
            else
            {
                existingCommentReplyEntity.Return();
                return existingCommentReplyEntity;
            }
        }
    }
}