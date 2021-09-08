using NexusMods.Monitor.Scraper.Domain.Events.Comments;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using NodaTime;

using System.Collections.Generic;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate
{
    public sealed record CommentEntity : Entity, IAggregateRoot
    {
        public uint NexusModsGameId { get; private set; }
        public uint NexusModsModId { get; private set; }
        public string GameName { get; private set; }
        public string ModName { get; private set; }
        public string Url { get; private set; }
        public string Author { get; private set; }
        public string AuthorUrl { get; private set; }
        public string AvatarUrl { get; private set; }
        public string Content { get; private set; }
        public bool IsSticky { get; private set; }
        public bool IsLocked { get; private set; }
        public bool IsDeleted { get; private set; }
        public Instant TimeOfPost { get; private set; }
        private readonly List<CommentReplyEntity> _replies = new();
        public IReadOnlyList<CommentReplyEntity> Replies => _replies.AsReadOnly();

        private CommentEntity() : this(default, default, default, default!, default!, default!, default!, default!, default!, default!, default, default, default, default) { }
        public CommentEntity(uint id, uint nexusModsGameId, uint nexusModsModId, string gameName, string modName, string url, string author, string authorUrl, string avatarUrl, string content, bool isSticky, bool isLocked, bool isDeleted, Instant timeOfPost) : base(id)
        {
            Id = id;
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
            IsDeleted = true;
            foreach (var reply in Replies)
            {
                reply.Remove();
            }
            AddDomainEvent(new CommentRemovedEvent(Id));
        }
        public void Return()
        {
            IsDeleted = false;
            AddDomainEvent(new CommentAddedEvent(Id));
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
        public void RemoveReply(uint id)
        {
            var commentReplyEntity = Replies.SingleOrDefault(o => o.Id == id);
            commentReplyEntity?.Remove();
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