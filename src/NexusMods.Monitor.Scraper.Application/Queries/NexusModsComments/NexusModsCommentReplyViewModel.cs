using AngleSharp.Dom;

using NexusMods.Monitor.Scraper.Application.Extensions;

using NodaTime;
using NodaTime.Text;

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    [DataContract]
    public sealed class NexusModsCommentReplyViewModel
    {
        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public uint OwnerId { get; private set; } = default!;
        [DataMember]
        public string Author { get; private set; } = default!;
        [DataMember]
        public string AuthorUrl { get; private set; } = default!;
        [DataMember]
        public string AvatarUrl { get; private set; } = default!;
        [DataMember]
        public string Content { get; private set; } = default!;
        [DataMember]
        public Instant Post { get; private set; } = default!;

        private NexusModsCommentReplyViewModel() { }
        public NexusModsCommentReplyViewModel(IElement element, uint ownerCommentId) : this()
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
            Post = InstantPattern.Create("dd MMMM yyyy, h:mmtt", CultureInfo.GetCultureInfo("en-UK")).Parse(time ?? "").GetValueOrThrow();
            Content = content?.GetElementsByClassName("comment-content-text").FirstOrDefault()?.ToText() ?? "ERROR";
        }

        public override string ToString() => Author;
    }
}