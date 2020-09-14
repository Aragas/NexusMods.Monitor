using NexusMods.Monitor.Scraper.Domain.Events.Comments;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using NodaTime;

using System.Collections.Generic;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate
{
    public sealed class CommentEntity : Entity, IAggregateRoot
    {
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;
        public string Url { get; private set; } = default!;
        public string Author { get; private set; } = default!;
        public string AuthorUrl { get; private set; } = default!;
        public string AvatarUrl { get; private set; } = default!;
        public string Content { get; private set; } = default!;
        public bool IsSticky { get; private set; } = default!;
        public bool IsLocked { get; private set; } = default!;
        public bool IsDeleted { get; private set; } = default!;
        public Instant TimeOfPost { get; private set; } = default!;

        private readonly List<CommentReplyEntity> _replies;
        public IEnumerable<CommentReplyEntity> Replies => _replies.AsReadOnly();

        private CommentEntity()
        {
            _replies = new List<CommentReplyEntity>();
        }

        public CommentEntity(uint id, uint nexusModsGameId, uint nexusModsModId, string url, string author, string authorUrl, string avatarUrl, string content, bool isSticky, bool isLocked, bool isDeleted, Instant timeOfPost) : this()
        {
            Id = id;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
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