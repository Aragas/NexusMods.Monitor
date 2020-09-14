using MediatR;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments;

using NodaTime;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    [DataContract]
    public sealed class CommentAddCommand : IRequest<bool>
    {
        [DataMember]
        private readonly List<CommentReplyDTO> _commentReplies;

        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsModId { get; private set; } = default!;
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
        public bool IsSticky { get; private set; } = default!;
        [DataMember]
        public bool IsLocked { get; private set; } = default!;
        [DataMember]
        public bool IsDeleted { get; private set; } = default!;
        [DataMember]
        public Instant TimeOfPost { get; private set; } = default!;

        [DataMember]
        public IEnumerable<CommentReplyDTO> CommentReplies => _commentReplies;

        private CommentAddCommand()
        {
            _commentReplies = new List<CommentReplyDTO>();
        }
        public CommentAddCommand(NexusModsCommentRootViewModel nexusModsCommentRoot) : this()
        {
            Id = nexusModsCommentRoot.NexusModsComment.Id;
            NexusModsGameId = nexusModsCommentRoot.NexusModsGameId;
            NexusModsModId = nexusModsCommentRoot.NexusModsModId;
            Url = $"https://www.nexusmods.com/{nexusModsCommentRoot.NexusModsGameIdText}/mods/{NexusModsModId}/?tab=posts&jump_to_comment={Id}";
            Author = nexusModsCommentRoot.NexusModsComment.Author;
            AuthorUrl = nexusModsCommentRoot.NexusModsComment.AuthorUrl;
            AvatarUrl = nexusModsCommentRoot.NexusModsComment.AvatarUrl;
            Content = nexusModsCommentRoot.NexusModsComment.Content;
            IsSticky = nexusModsCommentRoot.NexusModsComment.IsSticky;
            IsLocked = nexusModsCommentRoot.NexusModsComment.IsLocked;
            IsDeleted = false;
            TimeOfPost = nexusModsCommentRoot.NexusModsComment.Post;

            _commentReplies = nexusModsCommentRoot.NexusModsComment.Replies.Select(x => new CommentReplyDTO(
                x.Id,
                $"https://www.nexusmods.com/{nexusModsCommentRoot.NexusModsGameIdText}/mods/{NexusModsModId}/?tab=posts&jump_to_comment={x.Id}",
                x.Author,
                x.AuthorUrl,
                x.AvatarUrl,
                x.Content,
                false,
                x.Post
            )).ToList();
        }

        public sealed class CommentReplyDTO
        {
            public uint Id { get; private set; } = default!;
            public string Url { get; private set; } = default!;
            public string Author { get; private set; } = default!;
            public string AuthorUrl { get; private set; } = default!;
            public string AvatarUrl { get; private set; } = default!;
            public string Content { get; private set; } = default!;
            public bool IsDeleted { get; private set; } = default!;
            public Instant TimeOfPost { get; private set; } = default!;

            private CommentReplyDTO() { }
            public CommentReplyDTO(uint id, string url, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant time) : this()
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
}