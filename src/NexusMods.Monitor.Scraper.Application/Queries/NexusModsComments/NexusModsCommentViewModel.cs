using AngleSharp.Dom;

using NexusMods.Monitor.Scraper.Application.Extensions;
using NexusMods.Monitor.Shared.Domain;

using NodaTime;
using NodaTime.Text;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public sealed record NexusModsCommentViewModel(uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, bool IsSticky, bool IsLocked, Instant Post, IReadOnlyList<NexusModsCommentReplyViewModel> Replies)
    {
        public static NexusModsCommentViewModel FromElement(IElement element)
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

            return new NexusModsCommentViewModel(RecordUtils.Default<NexusModsCommentViewModel>())
            {
                Id = id,
                AuthorUrl = author?.GetAttribute("href") ?? "ERROR",
                AvatarUrl = author?.GetElementsByTagName("img").FirstOrDefault()?.GetAttribute("src") ?? "ERROR",
                Author = details?.GetElementsByClassName("comment-name").FirstOrDefault()?.ToText() ?? "ERROR",
                IsLocked = locked?.GetAttribute("style") is null || (locked.GetAttribute("style") is { } attr0 && string.IsNullOrEmpty(attr0)),
                IsSticky = sticky?.GetAttribute("style") is null || (sticky.GetAttribute("style") is { } attr1 && string.IsNullOrEmpty(attr1)),
                Post = InstantPattern.Create("dd MMMM yyyy, h:mmtt", CultureInfo.InvariantCulture).Parse(time ?? "").GetValueOrThrow(),
                Content = content?.GetElementsByClassName("comment-content-text").FirstOrDefault()?.ToText() ?? "ERROR",
                Replies = (kids?.Children ?? Enumerable.Empty<IElement>()).Select(subComment => NexusModsCommentReplyViewModel.FromElement(subComment, id)).ToImmutableArray(),
            };
        }

        public override string ToString() => Author;
    }
}