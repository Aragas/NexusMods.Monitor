using AngleSharp.Dom;

using NexusMods.Monitor.Metadata.Application.Extensions;
using NexusMods.Monitor.Shared.Common;

using NodaTime;
using NodaTime.Text;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NexusMods.Monitor.Metadata.Application.Queries.Comments
{
    public sealed record CommentViewModel(string GameDomain, uint GameId, uint ModId, string GameName, string ModName, uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, bool IsSticky, bool IsLocked, Instant Post, IReadOnlyList<CommentReplyViewModel> Replies)
    {
        public static CommentViewModel FromElement(string gameDomain, uint gameId, uint modId, string gameName, string modName, IElement element)
        {
            var commentIdSplit = element.Id?.Split("comment-", StringSplitOptions.RemoveEmptyEntries);
            var id = commentIdSplit?.Length > 0 ? uint.TryParse(commentIdSplit[0], out var commentId) ? commentId : uint.MaxValue : uint.MaxValue;
            var head = element.GetElementsByClassName("comment-head clearfix").FirstOrDefault();
            var author = head?.GetElementsByClassName("comment-user").FirstOrDefault();
            var details = head?.GetElementsByClassName("comment-details").FirstOrDefault();
            var content = element.GetElementsByClassName("comment-content").FirstOrDefault();
            var locked = content?.GetElementsByClassName("locked").FirstOrDefault();
            var sticky = content?.GetElementsByClassName("sticky").FirstOrDefault();
            var time = content?.GetElementsByTagName("time")?.FirstOrDefault()?.ToText();
            var kids = element.GetElementsByClassName("comment-kids").FirstOrDefault();

            return new CommentViewModel(RecordUtils.Default<CommentViewModel>())
            {
                GameDomain = gameDomain,
                GameId = gameId,
                ModId = modId,
                GameName = gameName,
                ModName = modName,
                Id = id,
                AuthorUrl = author?.GetAttribute("href") ?? "ERROR",
                AvatarUrl = author?.GetElementsByTagName("img").FirstOrDefault()?.GetAttribute("src") ?? "ERROR",
                Author = details?.GetElementsByClassName("comment-name").FirstOrDefault()?.ToText() ?? "ERROR",
                IsLocked = locked?.GetAttribute("style") is null,
                IsSticky = sticky?.GetAttribute("style") is null,
                Post = InstantPattern.CreateWithInvariantCulture("dd MMMM yyyy, h:mmtt").Parse(time ?? "").GetValueOrThrow(),
                Content = content?.GetElementsByClassName("comment-content-text").FirstOrDefault()?.ToText() ?? "ERROR",
                Replies = (kids?.Children ?? Enumerable.Empty<IElement>()).Select(subComment => CommentReplyViewModel.FromElement(subComment, id)).ToImmutableArray(),
            };
        }

        public override string ToString() => Author;
    }
}