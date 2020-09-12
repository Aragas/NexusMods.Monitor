using AngleSharp.Dom;

using NexusMods.Monitor.Scraper.Application.Extensions;

using NodaTime;
using NodaTime.Extensions;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public class NexusModsCommentViewModel
    {
        public uint Id { get; private set; } = default!;
        public string Author { get; private set; } = default!;
        public string AuthorUrl { get; private set; } = default!;
        public string AvatarUrl { get; private set; } = default!;
        public string Content { get; private set; } = default!;
        public bool IsSticky { get; private set; } = default!;
        public bool IsLocked { get; private set; } = default!;
        public Instant Post { get; private set; } = default!;

        private readonly List<NexusModsCommentReplyViewModel> _replies;
        public IEnumerable<NexusModsCommentReplyViewModel> Replies => _replies;

        private NexusModsCommentViewModel()
        {
            _replies = new List<NexusModsCommentReplyViewModel>();
        }
        public NexusModsCommentViewModel(IElement element) : this()
        {
            var commentIdSplit = element.Id.Split("comment-", StringSplitOptions.RemoveEmptyEntries);
            Id = commentIdSplit.Length > 0 ? uint.TryParse(commentIdSplit[0], out var commentId) ? commentId : uint.MaxValue : uint.MaxValue;

            var head = element.GetElementsByClassName("comment-head clearfix").FirstOrDefault();
            var author = head?.GetElementsByClassName("comment-user").FirstOrDefault();
            AuthorUrl = author?.GetAttribute("href") ?? "ERROR";
            AvatarUrl = author?.GetElementsByTagName("img").FirstOrDefault()?.GetAttribute("src") ?? "ERROR";
            var details = head?.GetElementsByClassName("comment-details").FirstOrDefault();
            Author = details?.GetElementsByClassName("comment-name").FirstOrDefault()?.ToText() ?? "ERROR";

            var content = element.GetElementsByClassName("comment-content").FirstOrDefault();
            var locked = content?.GetElementsByClassName("locked").FirstOrDefault();
            IsLocked = locked?.GetAttribute("style") is null || (locked.GetAttribute("style") is { } attr0 && string.IsNullOrEmpty(attr0));
            var sticky = content?.GetElementsByClassName("sticky").FirstOrDefault();
            IsSticky = sticky?.GetAttribute("style") is null || (sticky.GetAttribute("style") is { } attr1 && string.IsNullOrEmpty(attr1));
            var time = content?.GetElementsByTagName("time")?.FirstOrDefault()?.ToText();
            Post = DateTimeOffset.ParseExact(time, "dd MMMM yyyy, h:mmtt", CultureInfo.GetCultureInfo("en-UK")).ToInstant();
            Content = content?.GetElementsByClassName("comment-content-text").FirstOrDefault()?.ToText() ?? "ERROR";

            var kids = element.GetElementsByClassName("comment-kids").FirstOrDefault();
            foreach (var subComment in kids?.Children ?? Enumerable.Empty<IElement>())
                _replies.Add(new NexusModsCommentReplyViewModel(subComment, Id));
        }

        public override string ToString() => Author;
    }
}