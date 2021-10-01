using AngleSharp.Dom;
using AngleSharp.Html.Dom;

using NexusMods.Monitor.Metadata.Application.Extensions;
using NexusMods.Monitor.Shared.Common;

using NodaTime;
using NodaTime.Text;

using System;
using System.Linq;

namespace NexusMods.Monitor.Metadata.Application.Queries.Comments
{
    public sealed record CommentReplyViewModel(uint Id, uint OwnerId, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant Post)
    {
        public static CommentReplyViewModel FromElement(IElement element, uint ownerCommentId)
        {
            var commentIdSplit = element.Id?.Split("comment-", StringSplitOptions.RemoveEmptyEntries);
            var id = commentIdSplit?.Length > 0 ? uint.TryParse(commentIdSplit[0], out var commentId) ? commentId : uint.MaxValue : uint.MaxValue;
            var head = element.GetElementsByClassName("comment-head clearfix").FirstOrDefault() as IHtmlDivElement;
            var author = head?.GetElementsByClassName("comment-user").FirstOrDefault() as IHtmlAnchorElement;
            var authorImg = author?.FindChild<IHtmlImageElement>();
            var details = head?.GetElementsByClassName("comment-details").FirstOrDefault() as IHtmlDivElement;
            var content = element.GetElementsByClassName("comment-content").FirstOrDefault() as IHtmlDivElement;
            var time = content?.GetElementsByTagName("time")?.FirstOrDefault()?.ToText();

            return new CommentReplyViewModel(RecordUtils.Default<CommentReplyViewModel>())
            {
                OwnerId = ownerCommentId,
                Id = id,
                AuthorUrl = author?.Href ?? "ERROR",
                AvatarUrl = authorImg?.Source ?? "ERROR",
                Author = details?.GetElementsByClassName("comment-name").FirstOrDefault()?.ToText() ?? "ERROR",
                Post = InstantPattern.CreateWithInvariantCulture("dd MMMM yyyy, h:mmtt").Parse(time ?? "").GetValueOrThrow(),
                Content = content?.GetElementsByClassName("comment-content-text").FirstOrDefault()?.ToText() ?? "ERROR",
            };
        }

        public override string ToString() => Author;
    }
}