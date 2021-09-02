using MediatR;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments;

using NodaTime;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    // TODO:
    [DataContract]
    public sealed record CommentAddNewCommand : IRequest<bool>
    {
        [DataMember]
        private readonly IReadOnlyList<CommentReplyDTO> _commentReplies;

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

        private CommentAddNewCommand()
        {
            _commentReplies = new List<CommentReplyDTO>();
        }
        public CommentAddNewCommand(NexusModsCommentRootViewModel nexusModsCommentRoot) : this()
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

            _commentReplies = nexusModsCommentRoot.NexusModsComment.Replies.Select(x => new CommentReplyDTO
            (
                x.Id,
                $"https://www.nexusmods.com/{nexusModsCommentRoot.NexusModsGameIdText}/mods/{NexusModsModId}/?tab=posts&jump_to_comment={x.Id}",
                x.Author,
                x.AuthorUrl,
                x.AvatarUrl,
                x.Content,
                x.Post
            )).ToImmutableArray();
        }

        public sealed record CommentReplyDTO(uint Id, string Url, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant TimeOfPost);
    }
}