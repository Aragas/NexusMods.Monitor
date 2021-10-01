using NexusMods.Monitor.Scraper.Domain.Events.Comments;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using NodaTime;

using System.Collections.Generic;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate
{
    public sealed record CommentEntity(uint Id) : Entity(Id), IAggregateRoot
    {
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;
        public string GameName { get; private set; } = default!;
        public string ModName { get; private set; } = default!;
        public string Url { get; private set; } = default!;
        public string Author { get; private set; } = default!;
        public string AuthorUrl { get; private set; } = default!;
        public string AvatarUrl { get; private set; } = default!;
        public string Content { get; private set; } = default!;
        public bool IsSticky { get; private set; } = default!;
        public bool IsLocked { get; private set; } = default!;
        public bool IsDeleted { get; private set; } = default!;
        public Instant TimeOfPost { get; private set; } = default!;

        private readonly List<CommentReplyEntity> _replies = new();
        public IReadOnlyList<CommentReplyEntity> Replies => _replies.AsReadOnly();

        private CommentEntity() : this(default, default, default, default!, default!, default!, default!, default!, default!, default!, default, default, default, default) { }
        public CommentEntity(uint id, uint nexusModsGameId, uint nexusModsModId, string gameName, string modName, string url, string author, string authorUrl, string avatarUrl, string content, bool isSticky, bool isLocked, bool isDeleted, Instant timeOfPost) : this(id)
        {
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            GameName = gameName;
            ModName = modName;
            Url = url;
            Author = author;
            AuthorUrl = authorUrl;
            AvatarUrl = avatarUrl;
            Content = content;
            IsSticky = isSticky;
            IsLocked = isLocked;
            IsDeleted = isDeleted;
            TimeOfPost = timeOfPost;

            AddDomainEvent(new CommentAddedEvent(Id));
        }

        public void Remove()
        {
            if (IsDeleted != true)
            {
                AddDomainEvent(new CommentRemovedEvent(Id));
                IsDeleted = true;
                foreach (var reply in Replies)
                {
                    reply.Remove();
                }
            }
        }
        public void Return()
        {
            if (IsDeleted != false)
            {
                AddDomainEvent(new CommentAddedEvent(Id));
                IsDeleted = false;
            }
        }

        public CommentReplyEntity AddReplyEntity(uint id, string url, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant timeOfPost)
        {
            var existingCommentReplyEntity = Replies.SingleOrDefault(o => o.Id == id);
            if (existingCommentReplyEntity is null)
            {
                var commentReplyEntity = new CommentReplyEntity(id, Id, url, author, authorUrl, avatarUrl, content, isDeleted, timeOfPost);
                _replies.Add(commentReplyEntity);
                return commentReplyEntity;
            }
            else
            {
                existingCommentReplyEntity.Return();
                return existingCommentReplyEntity;
            }
        }
        public CommentReplyEntity? RemoveReplyEntity(uint id)
        {
            var commentReplyEntity = Replies.SingleOrDefault(o => o.Id == id);
            if (commentReplyEntity is not null)
            {
                commentReplyEntity.Remove();
                return commentReplyEntity;
            }
            return null;
        }

        public void SetIsSticky(bool isSticky)
        {
            if (IsSticky != isSticky)
            {
                AddDomainEvent(new CommentChangedIsStickyEvent(Id, IsSticky));
                IsSticky = isSticky;
            }
        }

        public void SetIsLocked(bool isLocked)
        {
            if (IsLocked != isLocked)
            {
                AddDomainEvent(new CommentChangedIsLockedEvent(Id, IsLocked));
                IsLocked = isLocked;
            }
        }
    }
}