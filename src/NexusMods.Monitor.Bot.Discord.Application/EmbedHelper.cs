using Discord;

using NexusMods.Monitor.Bot.Discord.Application.Queries.RateLimits;
using NexusMods.Monitor.Shared.Application.Models;

using NodaTime;

using System;
using System.Reflection;

namespace NexusMods.Monitor.Bot.Discord.Application
{
    public static class EmbedHelper
    {
        private static readonly Color Color = new(218, 142, 53);
        private static readonly int MaxTextLength = 400;

        public static Embed About(int serverCount, int subscriptionCount, Duration uptime) => new EmbedBuilder()
            .WithTitle("Nexus Mods Monitor (Unofficial)")
            .WithDescription(@$"**Version:** {Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "UNKNOWN"}
**Source: **[GitHub](https://github.com/Aragas/NexusMods.Monitor)

Gives the ability to subscribe to your mod page notifications. Posts and Bugs sections are supported.")
            .WithThumbnailUrl("https://cdn.discordapp.com/app-icons/751048410357956658/168781156967a40bba1362042f7f1713.png")
            .WithCurrentTimestamp()
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Support")
                    .WithValue("Contact author Aragas#7671 for support!")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Stats")
                    .WithValue(@$"Servers: {serverCount}
Subscriptions: {subscriptionCount}"))
            .WithFooter(
                $"Uptime: {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s",
                "https://cdn.discordapp.com/app-icons/751048410357956658/168781156967a40bba1362042f7f1713.png")
            .Build();

        public static Embed RateLimits(RateLimitViewModel rateLimit) => new EmbedBuilder()
            .WithTitle("API Rate Limits")
            .WithThumbnailUrl("https://cdn.discordapp.com/app-icons/751048410357956658/168781156967a40bba1362042f7f1713.png")
            .WithCurrentTimestamp()
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Site")
                    .WithValue($"Retry After: {rateLimit.SiteLimit.RetryAfter?.ToString("O") ?? "None"}"),
                new EmbedFieldBuilder()
                    .WithName("API")
                    .WithValue(@$"Hourly Remaining: {rateLimit.APILimit.HourlyRemaining}
Hourly Reset: {rateLimit.APILimit.HourlyReset}
Daily Remaining: {rateLimit.APILimit.DailyRemaining}
Daily Reset: {rateLimit.APILimit.DailyReset}"))
            .Build();

        public static Embed NewIssue(IssueDTO issue) => new EmbedBuilder()
            .WithTitle($"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: New report - '{issue.Title}'")
            .WithAuthor(issue.Content!.Author, issue.Content.AvatarUrl, issue.Content.AuthorUrl)
            .WithThumbnailUrl(issue.Content.AvatarUrl)
            .WithTimestamp(issue.TimeOfLastPost)
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new EmbedFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Message")
                    .WithValue(issue.Content.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Embed DeletedIssue(IssueDTO issue) => new EmbedBuilder()
            .WithTitle($"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report was deleted - '{issue.Title}'")
            .WithAuthor(issue.Content!.Author, issue.Content.AvatarUrl, issue.Content.AuthorUrl)
            .WithThumbnailUrl(issue.Content.AvatarUrl)
            .WithTimestamp(issue.TimeOfLastPost)
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new EmbedFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Message")
                    .WithValue(issue.Content.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Embed NewIssueReply(IssueDTO issue, IssueReplyDTO issueReply) => new EmbedBuilder()
            .WithTitle($"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: New reply - '{issue.Title}'")
            .WithDescription(issue.Status.Name)
            .WithAuthor(issueReply.Author, issueReply.AvatarUrl, issueReply.AuthorUrl)
            .WithThumbnailUrl(issueReply.AvatarUrl)
            .WithTimestamp(issueReply.TimeOfPost)
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new EmbedFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .WithFields(new EmbedFieldBuilder()
                .WithName("Message")
                .WithValue(issueReply.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Embed DeletedIssueReply(IssueDTO issue, IssueReplyDTO issueReply) => new EmbedBuilder()
            .WithTitle($"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Reply was deleted - '{issue.Title}'")
            .WithDescription(issue.Status.Name)
            .WithAuthor(issueReply.Author, issueReply.AvatarUrl, issueReply.AuthorUrl)
            .WithThumbnailUrl(issueReply.AvatarUrl)
            .WithCurrentTimestamp()
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new EmbedFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .WithFields(new EmbedFieldBuilder()
                .WithName("Message")
                .WithValue(issueReply.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Embed StatusChanged(IssueDTO issue, IssueStatusDTO oldIssueStatus) => new EmbedBuilder()
            .WithTitle($"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report status changed - '{issue.Title}'")
            .WithCurrentTimestamp()
            .WithDescription($"{oldIssueStatus.Name} -> {issue.Status.Name}")
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new EmbedFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .Build();

        public static Embed PriorityChanged(IssueDTO issue, IssuePriorityDTO oldIssuePriority) => new EmbedBuilder()
            .WithTitle($"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report priority changed - '{issue.Title}'")
            .WithCurrentTimestamp()
            .WithDescription($"{oldIssuePriority.Name} -> {issue.Priority.Name}")
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new EmbedFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .Build();

        public static Embed IsClosedChanged(IssueDTO issue) => new EmbedBuilder()
            .WithTitle(issue.IsClosed
                ? $"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report is closed - '{issue.Title}'"
                : $"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report is re-opened - '{issue.Title}'")
            .WithCurrentTimestamp()
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),

                new EmbedFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .Build();

        public static Embed IsPrivateChanged(IssueDTO issue) => new EmbedBuilder()
            .WithTitle(issue.IsPrivate
                ? $"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report is private - '{issue.Title}'"
                : $"Game: {issue.GameName}\nMod: {issue.ModName}\nBugs: Report is public - '{issue.Title}'")
            .WithCurrentTimestamp()
            .WithUrl(issue.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Status")
                    .WithValue(issue.Status.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issue.Priority.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issue.ModVersion)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Private")
                    .WithValue(issue.IsPrivate.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issue.IsClosed.ToString())
                    .WithIsInline(true))
            .Build();


        public static Embed NewComment(CommentDTO comment) => new EmbedBuilder()
            .WithTitle($"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: New post")
            .WithAuthor(comment.Author, comment.AvatarUrl, comment.AuthorUrl)
            .WithThumbnailUrl(comment.AvatarUrl)
            .WithTimestamp(comment.TimeOfPost)
            .WithUrl(comment.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Sticky")
                    .WithValue(comment.IsSticky.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Locked")
                    .WithValue(comment.IsLocked.ToString())
                    .WithIsInline(true))
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Message")
                    .WithValue(comment.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Embed DeletedComment(CommentDTO comment) => new EmbedBuilder()
            .WithTitle($"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: Post was deleted")
            .WithAuthor(comment.Author, comment.AvatarUrl, comment.AuthorUrl)
            .WithThumbnailUrl(comment.AvatarUrl)
            .WithTimestamp(comment.TimeOfPost)
            .WithUrl(comment.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Sticky")
                    .WithValue(comment.IsSticky.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Locked")
                    .WithValue(comment.IsLocked.ToString())
                    .WithIsInline(true))
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Message")
                    .WithValue(comment.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Embed NewCommentReply(CommentDTO comment, CommentReplyDTO commentReply) => new EmbedBuilder()
            .WithTitle($"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: New reply")
            .WithAuthor(commentReply.Author, commentReply.AvatarUrl, commentReply.AuthorUrl)
            .WithThumbnailUrl(commentReply.AvatarUrl)
            .WithTimestamp(commentReply.TimeOfPost)
            .WithUrl(commentReply.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Sticky")
                    .WithValue(comment.IsSticky.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Locked")
                    .WithValue(comment.IsLocked.ToString())
                    .WithIsInline(true))
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Message")
                    .WithValue(commentReply.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Embed DeletedCommentReply(CommentDTO comment, CommentReplyDTO commentReply) => new EmbedBuilder()
            .WithTitle($"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: Reply was deleted")
            .WithAuthor(commentReply.Author, commentReply.AvatarUrl, commentReply.AuthorUrl)
            .WithThumbnailUrl(commentReply.AvatarUrl)
            .WithTimestamp(commentReply.TimeOfPost)
            .WithUrl(commentReply.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Sticky")
                    .WithValue(comment.IsSticky.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Locked")
                    .WithValue(comment.IsLocked.ToString())
                    .WithIsInline(true))
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Message")
                    .WithValue(commentReply.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Embed IsLockedChanged(CommentDTO comment) => new EmbedBuilder()
            .WithTitle(comment.IsLocked
                ? $"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: Post is locked"
                : $"Game: {comment.GameName}\nMod: {comment.ModName}\nPosts: Post is unlocked")
            .WithCurrentTimestamp()
            .WithUrl(comment.Url)
            .WithColor(Color)
            .Build();

        public static Embed IsStickyChanged(CommentDTO comment) => new EmbedBuilder()
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