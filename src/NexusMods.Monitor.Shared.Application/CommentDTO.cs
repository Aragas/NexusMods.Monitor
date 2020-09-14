using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NexusMods.Monitor.Shared.Application
{
    [DataContract]
    public sealed class CommentDTO
    {
        [DataMember]
        public uint Id { get; private set;} = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set;} = default!;
        [DataMember]
        public uint NexusModsModId { get; private set;} = default!;
        [DataMember]
        public string Url { get; private set;} = default!;
        [DataMember]
        public string Author { get; private set; } = default!;
        [DataMember]
        public string AuthorUrl { get; private set;} = default!;
        [DataMember]
        public string AvatarUrl { get; private set;} = default!;
        [DataMember]
        public string Content { get; private set;} = default!;
        [DataMember]
        public bool IsSticky { get; private set; } = default!;
        [DataMember]
        public bool IsLocked { get; private set; } = default!;
        [DataMember]
        public DateTimeOffset TimeOfPost { get; private set;} = default!;

        public IReadOnlyCollection<CommentReplyDTO> Replies  { get; private set;} = default!;

        private CommentDTO()
        {
            Replies = new List<CommentReplyDTO>();
        }
        public CommentDTO(uint id, uint nexusModsGameId, uint nexusModsModId, string url, string author, string authorUrl, string avatarUrl, string content, bool isSticky, bool isLocked, DateTimeOffset timeOfPost, IEnumerable<CommentReplyDTO> replies)
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
            TimeOfPost = timeOfPost;
            Replies = replies.ToList();
        }
    }

    [DataContract]
    public sealed class CommentReplyDTO
    {
        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public string Url { get; private set; } = default!;
        [DataMember]
        public string Author { get; private set; } = default!;
        [DataMember]
        public string AuthorUrl { get; private set; } = default!;
        [DataMember]
        public string AvatarUrl { get; private set; } = default!;
        [DataMember]
        public string Content { get; private set; } = default!;
        [DataMember]
        public bool IsDeleted { get; private set; } = default!;
        [DataMember]
        public DateTimeOffset TimeOfPost { get; private set; } = default!;

        private CommentReplyDTO() { }
        public CommentReplyDTO(uint id, string url, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, DateTimeOffset time) : this()
        {
            Id = id;
            Url = url;
            Author = author;
            AuthorUrl = authorUrl;
            AvatarUrl = avatarUrl;
            Content = content;
            IsDeleted = isDeleted;
            TimeOfPost = time;
        }
    }
}