using AngleSharp.Dom;

using NexusMods.Monitor.Metadata.Application.Extensions;
using NexusMods.Monitor.Shared.Common;
using NexusMods.Monitor.Shared.Domain;

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
            var head = element.GetElementsByClassName("comment-head clearfix").FirstOrDefault();
            var author = head?.GetElementsByClassName("comment-user").FirstOrDefault();
            var details = head?.GetElementsByClassName("comment-details").FirstOrDefault();
            var content = element.GetElementsByClassName("comment-content").FirstOrDefault();
            var time = content?.GetElementsByTagName("time")?.FirstOrDefault()?.ToText();

            return new CommentReplyViewModel(RecordUtils.Default<CommentReplyViewModel>())
            {
                OwnerId = ownerCommentId,
                Id = commentIdSplit?.Length > 0 ? uint.TryParse(commentIdSplit[0], out var commentId) ? commentId : uint.MaxValue : uint.MaxValue,
                AuthorUrl = author?.GetAttribute("href") ?? "ERROR",
                AvatarUrl = author?.GetElementsByTagName("img").FirstOrDefault()?.GetAttribute("src") ?? "ERROR",
                Author = details?.GetElementsByClassName("comment-name").FirstOrDefault()?.ToText() ?? "ERROR",
                Post = InstantPattern.CreateWithInvariantCulture("dd MMMM yyyy, h:mmtt").Parse(time ?? "").GetValueOrThrow(),
                Content = content?.GetElementsByClassName("comment-content-text").FirstOrDefault()?.ToText() ?? "ERROR",
            };
        }

        public override string ToString() => Author;
    }
}