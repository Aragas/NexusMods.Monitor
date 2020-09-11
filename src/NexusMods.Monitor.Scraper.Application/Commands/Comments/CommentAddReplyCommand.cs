using MediatR;

using NexusMods.Monitor.Scraper.Infrastructure.Models.Comments;

using NodaTime;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    [DataContract]
    public class CommentAddReplyCommand : IRequest<bool>
    {
        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public uint ReplyId { get; private set; } = default!;
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
        public Instant TimeOfPost { get; private set; } = default!;

        private CommentAddReplyCommand() { }
        public CommentAddReplyCommand(NexusModsCommentRoot nexusModsCommentRoot, NexusModsCommentReply nexusModsCommentReply) : this()
        {
            Id = nexusModsCommentReply.OwnerId;
            ReplyId = nexusModsCommentReply.Id;
            Url = $"https://www.nexusmods.com/{nexusModsCommentRoot.NexusModsGameIdText}/mods/{nexusModsCommentRoot.NexusModsModId}/?tab=posts&jump_to_comment={Id}";
            Author = nexusModsCommentReply.Author;
            AuthorUrl = nexusModsCommentReply.AuthorUrl;
            AvatarUrl = nexusModsCommentReply.AvatarUrl;
            Content = nexusModsCommentReply.Content;
            IsDeleted = false;
            TimeOfPost = nexusModsCommentReply.Post;
        }
    }
}