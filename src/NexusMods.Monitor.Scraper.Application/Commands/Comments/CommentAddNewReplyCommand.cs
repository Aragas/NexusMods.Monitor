using MediatR;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments;

using NodaTime;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    // TODO:
    [DataContract]
    public sealed record CommentAddNewReplyCommand : IRequest<bool>
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
        public Instant TimeOfPost { get; private set; } = default!;

        private CommentAddNewReplyCommand() { }
        public CommentAddNewReplyCommand(NexusModsCommentRootViewModel nexusModsCommentRoot, NexusModsCommentReplyViewModel nexusModsCommentReply) : this()
        {
            Id = nexusModsCommentReply.OwnerId;
            ReplyId = nexusModsCommentReply.Id;
            Url = $"https://www.nexusmods.com/{nexusModsCommentRoot.NexusModsGameIdText}/mods/{nexusModsCommentRoot.NexusModsModId}/?tab=posts&jump_to_comment={Id}";
            Author = nexusModsCommentReply.Author;
            AuthorUrl = nexusModsCommentReply.AuthorUrl;
            AvatarUrl = nexusModsCommentReply.AvatarUrl;
            Content = nexusModsCommentReply.Content;
            TimeOfPost = nexusModsCommentReply.Post;
        }
    }
}