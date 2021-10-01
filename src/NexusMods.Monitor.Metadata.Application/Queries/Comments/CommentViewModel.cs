using AngleSharp.Dom;
using AngleSharp.Html.Dom;

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
            var head = element.GetElementsByClassName("comment-head clearfix").FirstOrDefault() as IHtmlDivElement;
            var author = head?.GetElementsByClassName("comment-user").FirstOrDefault() as IHtmlAnchorElement;
            var authorImg = author?.FindChild<IHtmlImageElement>();
            var details = head?.GetElementsByClassName("comment-details").FirstOrDefault() as IHtmlDivElement;
            var content = element.GetElementsByClassName("comment-content").FirstOrDefault() as IHtmlDivElement;
            var locked = content?.Children.FirstOrDefault(e => e.Id == $"locked-comment-label-{id}") as IHtmlDivElement;
            var sticky = content?.Children.FirstOrDefault(e => e.Id == $"sticky-comment-label-{id}") as IHtmlDivElement;
            var time = content?.GetElementsByTagName("time").FirstOrDefault()?.ToText();
            var kids = element.GetElementsByClassName("comment-kids").FirstOrDefault() as IHtmlOrderedListElement;

            return new CommentViewModel(RecordUtils.Default<CommentViewModel>())
            {
                GameDomain = gameDomain,
                GameId = gameId,
                ModId = modId,
                GameName = gameName,
                ModName = modName,
                Id = id,
                AuthorUrl = author?.Href ?? "ERROR",
                AvatarUrl = authorImg?.Source ?? "ERROR",
                Author = details?.GetElementsByClassName("comment-name").FirstOrDefault()?.ToText() ?? "ERROR",
                IsLocked = locked is not null && !locked.IsHidden(),
                IsSticky = sticky is not null && !sticky.IsHidden(),
                Post = InstantPattern.CreateWithInvariantCulture("dd MMMM yyyy, h:mmtt").Parse(time ?? "").GetValueOrThrow(),
                Content = content?.GetElementsByClassName("comment-content-text").FirstOrDefault()?.ToText() ?? "ERROR",
                Replies = (kids?.Children ?? Enumerable.Empty<IElement>()).Select(subComment => CommentReplyViewModel.FromElement(subComment, id)).ToImmutableArray(),
            };
        }

        public override string ToString() => Author;
    }
}