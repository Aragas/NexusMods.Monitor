using AngleSharp.Dom;

using NexusMods.Monitor.Scraper.Application.Extensions;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;

using NodaTime;
using NodaTime.Extensions;

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    [DataContract]
    public sealed class NexusModsIssueViewModel
    {
        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public string Title { get; private set; } = default!;
        [DataMember]
        public bool IsPrivate { get; private set; } = default!;
        [DataMember]
        public bool IsClosed { get; private set; } = default!;
        [DataMember]
        public IssueStatusEnumeration Status { get; private set; } = default!;
        [DataMember]
        public uint ReplyCount { get; private set; } = default!;
        [DataMember]
        public string ModVersion { get; private set; } = default!;
        [DataMember]
        public IssuePriorityEnumeration Priority { get; private set; } = default!;
        [DataMember]
        public Instant LastPost { get; private set; } = default!;

        private NexusModsIssueViewModel() { }
        public NexusModsIssueViewModel(IElement element) : this()
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