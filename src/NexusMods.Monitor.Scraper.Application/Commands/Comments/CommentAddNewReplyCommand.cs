using MediatR;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments;
using NexusMods.Monitor.Shared.Common;

using NodaTime;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    public sealed record CommentAddNewReplyCommand(uint Id, uint ReplyId, string Url, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant TimeOfPost) : IRequest<bool>
    {
        public static CommentAddNewReplyCommand FromViewModel(NexusModsCommentRootViewModel nexusModsCommentRoot, NexusModsCommentReplyViewModel nexusModsCommentReply)
        {
            return new CommentAddNewReplyCommand(RecordUtils.Default<CommentAddNewReplyCommand>())
            {
                Id = nexusModsCommentReply.OwnerId,
                ReplyId = nexusModsCommentReply.Id,
                Url = $"https://www.nexusmods.com/{nexusModsCommentRoot.GameDomain}/mods/{nexusModsCommentRoot.ModId}/?tab=posts&jump_to_comment={nexusModsCommentReply.OwnerId}",
                Author = nexusModsCommentReply.Author,
                AuthorUrl = nexusModsCommentReply.AuthorUrl,
                AvatarUrl = nexusModsCommentReply.AvatarUrl,
                Content = nexusModsCommentReply.Content,
                TimeOfPost = nexusModsCommentReply.Post,
            };
        }
    }
}