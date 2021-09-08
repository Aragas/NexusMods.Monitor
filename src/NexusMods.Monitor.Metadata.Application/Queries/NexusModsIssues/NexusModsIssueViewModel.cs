using AngleSharp.Dom;

using NexusMods.Monitor.Metadata.Application.Extensions;
using NexusMods.Monitor.Shared.Domain;

using NodaTime;
using NodaTime.Extensions;

using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace NexusMods.Monitor.Metadata.Application.Queries.NexusModsIssues
{
    public sealed record NexusModsIssueStatus(uint Id, string Name)
    {
        public static readonly NexusModsIssueStatus None = new(1, "ERROR");
        public static readonly NexusModsIssueStatus NewIssue = new(2, "New Issue");
        public static readonly NexusModsIssueStatus BeingLookedAt = new(3, "Being Looked At");
        public static readonly NexusModsIssueStatus Fixed = new(4, "Fixed");
        public static readonly NexusModsIssueStatus KnownIssue = new(5, "Known Issue");
        public static readonly NexusModsIssueStatus Duplicate = new(6, "Duplicate");
        public static readonly NexusModsIssueStatus NotABug = new(7, "Not a Bug");
        public static readonly NexusModsIssueStatus WontFix = new(8, "Won't Fix");
        public static readonly NexusModsIssueStatus NeedsMoreInfo = new(9, "Needs More Info");
    }

    public sealed record NexusModsIssuePriority(uint Id, string Name)
    {
        public static readonly NexusModsIssuePriority None = new(1, "ERROR");
        public static readonly NexusModsIssuePriority NotSet = new(2, "Not Set");
        public static readonly NexusModsIssuePriority Low = new(3, "Low");
        public static readonly NexusModsIssuePriority Medium = new(4, "Medium");
        public static readonly NexusModsIssuePriority High = new(5, "High");
    }

    public sealed record NexusModsIssueViewModel(uint Id, string Title, bool IsPrivate, bool IsClosed, NexusModsIssueStatus Status, uint ReplyCount, string ModVersion, NexusModsIssuePriority Priority, Instant LastPost)
    {
        public static NexusModsIssueViewModel FromElement(IElement element)
        {
            var flags = (element.GetElementsByClassName("table-bug-title").FirstOrDefault()?.GetElementsByClassName("forum-sticky") ?? Enumerable.Empty<IElement>()).ToImmutableArray();
            var lastPost = element.GetElementsByClassName("table-bug-post").FirstOrDefault()?.ToText() ?? "01 Jan 2000 0:01AM";

            return new NexusModsIssueViewModel(RecordUtils.Default<NexusModsIssueViewModel>())
            {
                Id = uint.TryParse(element.GetAttribute("data-issue-id"), out var id) ? id : uint.MaxValue,
                Title = element.GetElementsByClassName("table-bug-title").FirstOrDefault()?.GetElementsByClassName("issue-title").FirstOrDefault()?.ToText() ?? "ERROR",
                IsPrivate = flags.Any(f => f.ToText() == "Private" && (f.GetAttribute("style") is null)),
                IsClosed = flags.Any(f => f.ToText() == "Closed" && (f.GetAttribute("style") is null)),
                Status = element.GetElementsByClassName("table-bug-status").FirstOrDefault()?.ToText() switch
                {
                    "New issue" => NexusModsIssueStatus.NewIssue,
                    "Being looked at" => NexusModsIssueStatus.BeingLookedAt,
                    "Fixed" => NexusModsIssueStatus.Fixed,
                    "Known issue" => NexusModsIssueStatus.KnownIssue,
                    "Duplicate" => NexusModsIssueStatus.Duplicate,
                    "Not a bug" => NexusModsIssueStatus.NotABug,
                    "Won't fix" => NexusModsIssueStatus.WontFix,
                    "Needs more info" => NexusModsIssueStatus.NeedsMoreInfo,
                    _ => NexusModsIssueStatus.None
                },
                ReplyCount = uint.TryParse(element.GetElementsByClassName("table-bug-replies").FirstOrDefault()?.ToText(), out var replies) ? replies : uint.MaxValue,
                ModVersion = element.GetElementsByClassName("table-bug-version").FirstOrDefault()?.ToText() ?? "ERROR",
                Priority = element.GetElementsByClassName("table-bug-priority").FirstOrDefault()?.ToText() switch
                {
                    "Not set" => NexusModsIssuePriority.NotSet,
                    "Low" => NexusModsIssuePriority.Low,
                    "Medium" => NexusModsIssuePriority.Medium,
                    "High" => NexusModsIssuePriority.High,
                    _ => NexusModsIssuePriority.None
                },
                LastPost = DateTimeOffset.ParseExact(lastPost, "dd MMM yyyy, h:mmtt", CultureInfo.InvariantCulture).ToInstant(),
            };
        }

        public override string ToString() => $"[{Status}] {Title}";
    }
}