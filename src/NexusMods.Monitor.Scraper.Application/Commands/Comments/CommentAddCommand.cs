using MediatR;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments;
using NexusMods.Monitor.Shared.Application;

using NodaTime;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    public sealed record CommentAddCommand(uint Id, uint NexusModsGameId, uint NexusModsModId, string Url, string Author, string AuthorUrl, string AvatarUrl, string Content, bool IsSticky, bool IsLocked, bool IsDeleted, Instant TimeOfPost, IReadOnlyList<CommentAddCommand.CommentReplyDTO> CommentReplies) : IRequest<bool>
    {
        public static CommentAddCommand FromViewModel(NexusModsCommentRootViewModel viewModel)
        {
            var id = viewModel.NexusModsComment.Id;
            var nexusModsModId = viewModel.NexusModsModId;

            return new CommentAddCommand(RecordUtils.Default<CommentAddCommand>())
            {
                Id = id,
                NexusModsGameId = viewModel.NexusModsGameId,
                NexusModsModId = nexusModsModId,
                Url = $"https://www.nexusmods.com/{viewModel.NexusModsGameIdText}/mods/{nexusModsModId}/?tab=posts&jump_to_comment={id}",
                Author = viewModel.NexusModsComment.Author,
                AuthorUrl = viewModel.NexusModsComment.AuthorUrl,
                AvatarUrl = viewModel.NexusModsComment.AvatarUrl,
                Content = viewModel.NexusModsComment.Content,
                IsSticky = viewModel.NexusModsComment.IsSticky,
                IsLocked = viewModel.NexusModsComment.IsLocked,
                IsDeleted = false,
                TimeOfPost = viewModel.NexusModsComment.Post,
                CommentReplies = viewModel.NexusModsComment.Replies.Select(x => new CommentReplyDTO(
                    x.Id,
                    $"https://www.nexusmods.com/{viewModel.NexusModsGameIdText}/mods/{nexusModsModId}/?tab=posts&jump_to_comment={x.Id}",
                    x.Author,
                    x.AuthorUrl,
                    x.AvatarUrl,
                    x.Content,
                    false,
                    x.Post)).ToImmutableArray(),
            };
        }

        public sealed record CommentReplyDTO(uint Id, string Url, string Author, string AuthorUrl, string AvatarUrl, string Content, bool IsDeleted, Instant TimeOfPost);
    }
}