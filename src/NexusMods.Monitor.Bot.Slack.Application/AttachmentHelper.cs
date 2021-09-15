using NexusMods.Monitor.Bot.Slack.Application.Queries.RateLimits;
using NexusMods.Monitor.Shared.Application;
using NexusMods.Monitor.Shared.Application.Models;

using NodaTime;

using SlackNet;

using System;
using System.Drawing;
using System.Reflection;

namespace NexusMods.Monitor.Bot.Slack.Application
{
    public static class AttachmentHelper
    {
        private static readonly Color Color = Color.FromArgb(255, 218, 142, 53);
        private static readonly int MaxTextLength = 400;


        public static Attachment About(int subscriptionCount, Duration uptime) => new AttachmentBuilder()
            .WithTitle("Nexus Mods Monitor (Unofficial)")
            .WithDescription(@$"Version: {Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "UNKNOWN"}

Gives the ability to subscribe to your mod page notifications. Posts and Bugs sections are supported.")
            .WithThumbnailUrl("https://cdn.discordapp.com/app-icons/751048410357956658/168781156967a40bba1362042f7f1713.png")
            .WithCurrentTimestamp()
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Support")
                    .WithValue("Contact author Aragas for support!")
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Stats")
                    .WithValue($"Subscriptions: {subscriptionCount}"))
            .WithFooter(
                $"Uptime: {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s",
                "https://cdn.discordapp.com/app-icons/751048410357956658/168781156967a40bba1362042f7f1713.png")
            .Build();

        public static Attachment RateLimits(RateLimitViewModel rateLimit) => new AttachmentBuilder()
            .WithTitle("API Rate Limits")
            .WithThumbnailUrl("https://cdn.discordapp.com/app-icons/751048410357956658/168781156967a40bba1362042f7f1713.png")
            .WithCurrentTimestamp()
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Site")
                    .WithValue($"Retry After: {rateLimit.SiteLimit.RetryAfter?.ToString("O") ?? "None"}"),
                new AttachmentFieldBuilder()
                    .WithName("API")
                    .WithValue(@$"Hourly Remaining: {rateLimit.APILimit.HourlyRemaining}
Hourly Reset: {rateLimit.APILimit.HourlyReset}
Daily Remaining: {rateLimit.APILimit.DailyRemaining}
Daily Reset: {rateLimit.APILimit.DailyReset}"))
            .Build();

        public static Attachment NewIssue(IssueDTO issue) => new AttachmentBuilder()
            .WithTitle($"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: New report - '{issue.Title}'")
            .WithAuthor(issue.Content!.Author, issue.Content.AvatarUrl, issue.Content.AuthorUrl)
            .WithThumbnailUrl(issue.Content.AvatarUrl)
            .WithTimestamp(issue.TimeOfLastPost)
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new AttachmentFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Message")
                    .WithValue(issue.Content.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Attachment DeletedIssue(IssueDTO issue) => new AttachmentBuilder()
            .WithTitle($"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report was deleted - '{issue.Title}'")
            .WithAuthor(issue.Content!.Author, issue.Content.AvatarUrl, issue.Content.AuthorUrl)
            .WithThumbnailUrl(issue.Content.AvatarUrl)
            .WithTimestamp(issue.TimeOfLastPost)
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new AttachmentFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Message")
                    .WithValue(issue.Content.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Attachment NewIssueReply(IssueDTO issue, IssueReplyDTO issueReply) => new AttachmentBuilder()
            .WithTitle($"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: New reply - '{issue.Title}'")
            .WithDescription(issue.Status.Name)
            .WithAuthor(issueReply.Author, issueReply.AvatarUrl, issueReply.AuthorUrl)
            .WithThumbnailUrl(issueReply.AvatarUrl)
            .WithTimestamp(issueReply.TimeOfPost)
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new AttachmentFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .WithFields(new AttachmentFieldBuilder()
                .WithName("Message")
                .WithValue(issueReply.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Attachment DeletedIssueReply(IssueDTO issue, IssueReplyDTO issueReply) => new AttachmentBuilder()
            .WithTitle($"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Reply was deleted - '{issue.Title}'")
            .WithDescription(issue.Status.Name)
            .WithAuthor(issueReply.Author, issueReply.AvatarUrl, issueReply.AuthorUrl)
            .WithThumbnailUrl(issueReply.AvatarUrl)
            .WithCurrentTimestamp()
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new AttachmentFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .WithFields(new AttachmentFieldBuilder()
                .WithName("Message")
                .WithValue(issueReply.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Attachment StatusChanged(IssueDTO issue, IssueStatusDTO oldIssueStatus) => new AttachmentBuilder()
            .WithTitle($"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report status changed - '{issue.Title}'")
            .WithCurrentTimestamp()
            .WithDescription($"{oldIssueStatus.Name} -> {issue.Status.Name}")
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new AttachmentFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .Build();

        public static Attachment PriorityChanged(IssueDTO issue, IssuePriorityDTO oldIssuePriority) => new AttachmentBuilder()
            .WithTitle($"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report priority changed - '{issue.Title}'")
            .WithCurrentTimestamp()
            .WithDescription($"{issue.NexusModsModId}\n{oldIssuePriority.Name} -> {issue.Priority.Name}")
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new AttachmentFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .Build();

        public static Attachment IsClosedChanged(IssueDTO issue) => new AttachmentBuilder()
            .WithTitle(issue.IsClosed
                ? $"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report is closed - '{issue.Title}'"
                : $"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report is re-opened - '{issue.Title}'")
            .WithCurrentTimestamp()
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new AttachmentFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .Build();

        public static Attachment IsPrivateChanged(IssueDTO issue) => new AttachmentBuilder()
            .WithTitle(issue.IsPrivate
                ? $"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report is private - '{issue.Title}'"
                : $"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report is public - '{issue.Title}'")
            .WithCurrentTimestamp()
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .Build();


        public static Attachment NewComment(CommentDTO comment) => new AttachmentBuilder()
            .WithTitle($"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: New post")
            .WithAuthor(comment.Author, comment.AvatarUrl, comment.AuthorUrl)
            .WithThumbnailUrl(comment.AvatarUrl)
            .WithTimestamp(comment.TimeOfPost)
            .WithUrl(comment.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Sticky")
                    .WithValue(comment.IsSticky.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Locked")
                    .WithValue(comment.IsLocked.ToString())
                    .WithIsInline(true))
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Message")
                    .WithValue(comment.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Attachment DeletedComment(CommentDTO comment) => new AttachmentBuilder()
            .WithTitle($"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: Post was deleted")
            .WithAuthor(comment.Author, comment.AvatarUrl, comment.AuthorUrl)
            .WithThumbnailUrl(comment.AvatarUrl)
            .WithTimestamp(comment.TimeOfPost)
            .WithUrl(comment.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Sticky")
                    .WithValue(comment.IsSticky.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Locked")
                    .WithValue(comment.IsLocked.ToString())
                    .WithIsInline(true))
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Message")
                    .WithValue(comment.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Attachment NewCommentReply(CommentDTO comment, CommentReplyDTO commentReply) => new AttachmentBuilder()
            .WithTitle($"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: New reply")
            .WithAuthor(commentReply.Author, commentReply.AvatarUrl, commentReply.AuthorUrl)
            .WithThumbnailUrl(commentReply.AvatarUrl)
            .WithTimestamp(commentReply.TimeOfPost)
            .WithUrl(commentReply.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Sticky")
                    .WithValue(comment.IsSticky.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Locked")
                    .WithValue(comment.IsLocked.ToString())
                    .WithIsInline(true))
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Message")
                    .WithValue(commentReply.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Attachment DeletedCommentReply(CommentDTO comment, CommentReplyDTO commentReply) => new AttachmentBuilder()
            .WithTitle($"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: Reply was deleted")
            .WithAuthor(commentReply.Author, commentReply.AvatarUrl, commentReply.AuthorUrl)
            .WithThumbnailUrl(commentReply.AvatarUrl)
            .WithTimestamp(commentReply.TimeOfPost)
            .WithUrl(commentReply.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Sticky")
                    .WithValue(comment.IsSticky.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Locked")
                    .WithValue(comment.IsLocked.ToString())
                    .WithIsInline(true))
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Message")
                    .WithValue(commentReply.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Attachment IsLockedChanged(CommentDTO comment) => new AttachmentBuilder()
            .WithTitle(comment.IsLocked
                ? $"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: Post is locked"
                : $"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: Post is unlocked")
            .WithCurrentTimestamp()
            .WithUrl(comment.Url)
            .WithColor(Color)
            .Build();

        public static Attachment IsStickyChanged(CommentDTO comment) => new AttachmentBuilder()
            .WithTitle(comment.IsSticky
                ? $"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: Post is pinned"
                : $"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: Post is unpinned")
            .WithCurrentTimestamp()
            .WithUrl(comment.Url)
            .WithColor(Color)
            .Build();

        private static string WithMaxLength(this string value, int maxLength) => value.Substring(0, Math.Min(value.Length, maxLength));
    }
}