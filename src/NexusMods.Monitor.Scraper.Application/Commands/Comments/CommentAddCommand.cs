using MediatR;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments;
using NexusMods.Monitor.Shared.Common;

using NodaTime;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    public sealed record CommentAddCommand(uint Id, uint NexusModsGameId, uint NexusModsModId, string GameName, string ModName, string Url, string Author, string AuthorUrl, string AvatarUrl, string Content, bool IsSticky, bool IsLocked, Instant TimeOfPost, IReadOnlyList<CommentAddCommand.CommentReplyDTO> Replies) : IRequest<bool>
    {
        public sealed record CommentReplyDTO(uint Id, string Url, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant TimeOfPost);

        public static CommentAddCommand FromViewModel(NexusModsCommentRootViewModel viewModel)
        {
            return new CommentAddCommand(RecordUtils.Default<CommentAddCommand>())
            {
                Id = viewModel.Comment.Id,
                NexusModsGameId = viewModel.GameId,
                NexusModsModId = viewModel.ModId,
                GameName = viewModel.GameName,
                ModName = viewModel.ModName,
                Url = $"https://www.nexusmods.com/{viewModel.GameDomain}/mods/{viewModel.ModId}/?tab=posts&jump_to_comment={viewModel.Comment.Id}",
                Author = viewModel.Comment.Author,
                AuthorUrl = viewModel.Comment.AuthorUrl,
                AvatarUrl = viewModel.Comment.AvatarUrl,
                Content = viewModel.Comment.Content,
                IsSticky = viewModel.Comment.IsSticky,
                IsLocked = viewModel.Comment.IsLocked,
                TimeOfPost = viewModel.Comment.Post,
                Replies = viewModel.Comment.Replies.Select(x => new CommentReplyDTO(
                    x.Id,
                    $"https://www.nexusmods.com/{viewModel.GameDomain}/mods/{viewModel.ModId}/?tab=posts&jump_to_comment={x.Id}",
                    x.Author,
                    x.AuthorUrl,
                    x.AvatarUrl,
                    x.Content,
                    x.Post)).ToImmutableArray(),
            };
        }
    }
}