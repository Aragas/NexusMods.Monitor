using AngleSharp.Dom;

using NexusMods.Monitor.Scraper.Application.Extensions;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;

using NodaTime;
using NodaTime.Extensions;

using System;
using System.Globalization;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public class NexusModsIssueViewModel
    {
        public uint Id { get; }
        public string Title { get; }
        public bool IsPrivate { get; }
        public bool IsClosed { get; }
        public IssueStatusEnumeration Status { get; }
        public uint ReplyCount { get; }
        public string ModVersion { get; }
        public IssuePriorityEnumeration Priority { get; }
        public Instant LastPost { get; }

        public NexusModsIssueViewModel(IElement element)
        {
            Id = uint.TryParse(element.GetAttribute("data-issue-id"), out var id) ? id : uint.MaxValue;
            Title = element.GetElementsByClassName("table-bug-title").FirstOrDefault()?.GetElementsByClassName("issue-title").FirstOrDefault()?.ToText() ?? "ERROR";
            var flags = (element.GetElementsByClassName("table-bug-title").FirstOrDefault()?.GetElementsByClassName("forum-sticky") ?? Enumerable.Empty<IElement>()).ToList();
            IsPrivate = flags.Any(f => f.ToText() == "Private" && (f.GetAttribute("style") is null || (f.GetAttribute("style") is { } attr && string.IsNullOrEmpty(attr))));
            IsClosed = flags.Any(f => f.ToText() == "Closed" && (f.GetAttribute("style") is null || (f.GetAttribute("style") is {} attr && string.IsNullOrEmpty(attr))));
            Status = element.GetElementsByClassName("table-bug-status").FirstOrDefault()?.ToText() switch
            {
                "New issue" => IssueStatusEnumeration.NewIssue,
                "Being looked at" => IssueStatusEnumeration.BeingLookedAt,
                "Fixed" => IssueStatusEnumeration.Fixed,
                "Known issue" => IssueStatusEnumeration.KnownIssue,
                "Duplicate" => IssueStatusEnumeration.Duplicate,
                "Not a bug" => IssueStatusEnumeration.NotABug,
                "Won't fix" => IssueStatusEnumeration.WontFix,
                "Needs more info" => IssueStatusEnumeration.NeedsMoreInfo,
                _ => IssueStatusEnumeration.None
            };
            ReplyCount = uint.TryParse(element.GetElementsByClassName("table-bug-replies").FirstOrDefault()?.ToText(), out var replies) ? replies : uint.MaxValue;
            ModVersion = element.GetElementsByClassName("table-bug-version").FirstOrDefault()?.ToText() ?? "ERROR";
            Priority = element.GetElementsByClassName("table-bug-priority").FirstOrDefault()?.ToText() switch
            {
                "Not set" => IssuePriorityEnumeration.NotSet,
                "Low" => IssuePriorityEnumeration.Low,
                "Medium" => IssuePriorityEnumeration.Medium,
                "High" => IssuePriorityEnumeration.High,
                _ => IssuePriorityEnumeration.None
            };
            var lastPost = element.GetElementsByClassName("table-bug-post").FirstOrDefault()?.ToText() ?? "01 Jan 2000 0:01AM";
            LastPost = DateTimeOffset.ParseExact(lastPost, "dd MMM yyyy, h:mmtt", CultureInfo.GetCultureInfo("en-UK")).ToInstant();
        }

        public override string ToString() => $"[{Status}] {Title}";
    }
}