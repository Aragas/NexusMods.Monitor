using AngleSharp.Dom;

using NexusMods.Monitor.Scraper.Infrastructure.Extensions;

using NodaTime;
using NodaTime.Extensions;

using System;
using System.Globalization;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Infrastructure.Models.Comments
{
    public class NexusModsCommentReply
    {
        public uint Id { get; }
        public uint OwnerId { get; }
        public string Author { get; }
        public string AuthorUrl { get; }
        public string AvatarUrl { get; }
        public string Content { get; }
        public Instant Post { get; }

        public NexusModsCommentReply(IElement element, uint ownerCommentId) // comment-container
        {
            OwnerId = ownerCommentId;

            var commentIdSplit = element.Id.Split("comment-", StringSplitOptions.RemoveEmptyEntries);
            Id = commentIdSplit.Length > 0 ? uint.TryParse(commentIdSplit[0], out var commentId) ? commentId : uint.MaxValue : uint.MaxValue;

            var head = element.GetElementsByClassName("comment-head clearfix").FirstOrDefault();
            var author = head?.GetElementsByClassName("comment-user").FirstOrDefault();
            AuthorUrl = author?.GetAttribute("href") ?? "ERROR";
            AvatarUrl = author?.GetElementsByTagName("img").FirstOrDefault()?.GetAttribute("src") ?? "ERROR";
            var details = head?.GetElementsByClassName("comment-details").FirstOrDefault();
            Author = details?.GetElementsByClassName("comment-name").FirstOrDefault()?.ToText() ?? "ERROR";

            var content = element.GetElementsByClassName("comment-content").FirstOrDefault();
            var time = content?.GetElementsByTagName("time")?.FirstOrDefault()?.ToText();
            Post = DateTimeOffset.ParseExact(time, "dd MMMM yyyy, h:mmtt", CultureInfo.GetCultureInfo("en-UK")).ToInstant();
            Content = content?.GetElementsByClassName("comment-content-text").FirstOrDefault()?.ToText() ?? "ERROR";
        }

        public override string ToString() => Author;
    }
}