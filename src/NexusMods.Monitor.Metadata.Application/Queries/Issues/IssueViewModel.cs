using AngleSharp.Dom;

using NexusMods.Monitor.Metadata.Application.Extensions;
using NexusMods.Monitor.Shared.Common;
using NexusMods.Monitor.Shared.Domain;

using NodaTime;
using NodaTime.Extensions;

using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace NexusMods.Monitor.Metadata.Application.Queries.Issues
{
    public sealed record IssueViewModel(string GameDomain, uint GameId, uint ModId, string GameName, string ModName, uint Id, string Title, bool IsPrivate, bool IsClosed, IssueStatusViewModel Status, uint ReplyCount, string ModVersion, IssuePriorityViewModel Priority, Instant LastPost)
    {
        public static IssueViewModel FromElement(string gameDomain, uint gameId, uint modId, string gameName, string modName, IElement element)
        {
            var flags = (element.GetElementsByClassName("table-bug-title").FirstOrDefault()?.GetElementsByClassName("forum-sticky") ?? Enumerable.Empty<IElement>()).ToImmutableArray();
            var lastPost = element.GetElementsByClassName("table-bug-post").FirstOrDefault()?.ToText() ?? "01 Jan 2000 0:01AM";

            return new IssueViewModel(RecordUtils.Default<IssueViewModel>())
            {
                GameDomain = gameDomain,
                GameId = gameId,
                ModId = modId,
                GameName = gameName,
                ModName = modName,
                Id = uint.TryParse(element.GetAttribute("data-issue-id"), out var id) ? id : uint.MaxValue,
                Title = element.GetElementsByClassName("table-bug-title").FirstOrDefault()?.GetElementsByClassName("issue-title").FirstOrDefault()?.ToText() ?? "ERROR",
                IsPrivate = flags.Any(f => f.ToText() == "Private" && (f.GetAttribute("style") is null)),
                IsClosed = flags.Any(f => f.ToText() == "Closed" && (f.GetAttribute("style") is null)),
                Status = element.GetElementsByClassName("table-bug-status").FirstOrDefault()?.ToText() switch
                {
                    "New issue" => IssueStatusViewModel.NewIssue,
                    "Being looked at" => IssueStatusViewModel.BeingLookedAt,
                    "Fixed" => IssueStatusViewModel.Fixed,
                    "Known issue" => IssueStatusViewModel.KnownIssue,
                    "Duplicate" => IssueStatusViewModel.Duplicate,
                    "Not a bug" => IssueStatusViewModel.NotABug,
                    "Won't fix" => IssueStatusViewModel.WontFix,
                    "Needs more info" => IssueStatusViewModel.NeedsMoreInfo,
                    _ => IssueStatusViewModel.None
                },
                ReplyCount = uint.TryParse(element.GetElementsByClassName("table-bug-replies").FirstOrDefault()?.ToText(), out var replies) ? replies : uint.MaxValue,
                ModVersion = element.GetElementsByClassName("table-bug-version").FirstOrDefault()?.ToText() ?? "ERROR",
                Priority = element.GetElementsByClassName("table-bug-priority").FirstOrDefault()?.ToText() switch
                {
                    "Not set" => IssuePriorityViewModel.NotSet,
                    "Low" => IssuePriorityViewModel.Low,
                    "Medium" => IssuePriorityViewModel.Medium,
                    "High" => IssuePriorityViewModel.High,
                    _ => IssuePriorityViewModel.None
                },
                LastPost = DateTimeOffset.ParseExact(lastPost, "dd MMM yyyy, h:mmtt", CultureInfo.InvariantCulture).ToInstant(),
            };
        }

        public override string ToString() => $"[{Status}] {Title}";
    }
}