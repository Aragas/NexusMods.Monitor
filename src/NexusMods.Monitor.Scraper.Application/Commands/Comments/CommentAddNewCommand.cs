using MediatR;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments;
using NexusMods.Monitor.Shared.Common;

using NodaTime;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    public sealed record CommentAddNewCommand(uint Id, uint NexusModsGameId, uint NexusModsModId, string GameName, string ModName, string Url, string Author, string AuthorUrl, string AvatarUrl, string Content, bool IsSticky, bool IsLocked, Instant TimeOfPost, IReadOnlyList<CommentAddNewCommand.CommentReplyDTO> CommentReplies) : IRequest<bool>
    {
        public sealed record CommentReplyDTO(uint Id, string Url, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant TimeOfPost);

        public static CommentAddNewCommand FromViewModel(NexusModsCommentRootViewModel viewModel)
        {
            return new CommentAddNewCommand(RecordUtils.Default<CommentAddNewCommand>())
            {
                Id = viewModel.NexusModsComment.Id,
                NexusModsGameId = viewModel.GameId,
                NexusModsModId = viewModel.ModId,
                GameName = viewModel.GameName,
                ModName = viewModel.ModName,
                Url = $"https://www.nexusmods.com/{viewModel.GameDomain}/mods/{viewModel.ModId}/?tab=posts&jump_to_comment={viewModel.NexusModsComment.Id}",
                Author = viewModel.NexusModsComment.Author,
                AuthorUrl = viewModel.NexusModsComment.AuthorUrl,
                AvatarUrl = viewModel.NexusModsComment.AvatarUrl,
                Content = viewModel.NexusModsComment.Content,
                IsSticky = viewModel.NexusModsComment.IsSticky,
                IsLocked = viewModel.NexusModsComment.IsLocked,
                TimeOfPost = viewModel.NexusModsComment.Post,
                CommentReplies = viewModel.NexusModsComment.Replies.Select(x => new CommentReplyDTO
                (
                    x.Id,
                    $"https://www.nexusmods.com/{viewModel.GameDomain}/mods/{viewModel.ModId}/?tab=posts&jump_to_comment={x.Id}",
                    x.Author,
                    x.AuthorUrl,
                    x.AvatarUrl,
                    x.Content,
                    x.Post
                )).ToImmutableArray(),
            };
        }
    }
}