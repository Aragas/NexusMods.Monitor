﻿using AngleSharp.Dom;

using NexusMods.Monitor.Scraper.Application.Extensions;
using NexusMods.Monitor.Shared.Domain;

using NodaTime;
using NodaTime.Extensions;

using System;
using System.Globalization;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public sealed record NexusModsIssueContentViewModel(uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant Time)
    {
        public static NexusModsIssueContentViewModel FromElement(IElement element)
        {
            var idSplit = element.Id?.Split("bug-issue-tile-", StringSplitOptions.RemoveEmptyEntries);
            var time = element.GetElementsByClassName("comment-content").FirstOrDefault()?.GetElementsByTagName("time").FirstOrDefault()?.ToText() ?? "01 January 2000 0:01AM";

            return new NexusModsIssueContentViewModel(RecordUtils.Default<NexusModsIssueContentViewModel>())
            {
                Id = idSplit?.Length > 0 ? uint.TryParse(idSplit[0], out var id) ? id : uint.MaxValue : uint.MaxValue,
                Author = element.GetElementsByClassName("comment-name").FirstOrDefault()?.ToText() ?? "ERROR",
                AuthorUrl = element.GetElementsByClassName("comment-name").FirstOrDefault()?.GetElementsByTagName("a").FirstOrDefault()?.GetAttribute("href") ?? "ERROR",
                AvatarUrl = element.GetElementsByClassName("comment-user").FirstOrDefault()?.GetElementsByTagName("img").FirstOrDefault()?.GetAttribute("src") ?? "ERROR",
                Content = element.GetElementsByClassName("comment-content").FirstOrDefault()?.ToText() ?? "ERROR",
                //Content = string.Join('\n', Content.Split('\n').Skip(1)),
                Time = DateTimeOffset.ParseExact(time, "dd MMMM yyyy, h:mmtt", CultureInfo.GetCultureInfo("en-UK")).ToInstant(),
            };
        }

        public override string ToString() => Author;
    }
}