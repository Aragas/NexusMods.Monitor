using Discord;

using NexusMods.Monitor.Shared.Application;

using NodaTime;

using System;
using System.Reflection;

namespace NexusMods.Monitor.Bot.Discord.Application
{
    public static class EmbedHelper
    {
        private static readonly Color Color = new Color(218, 142, 53);
        private static readonly int MaxTextLength = 400;

        public static Embed About(int serverCount, int subscriptionCount, Duration uptime) => new EmbedBuilder()
            .WithTitle("Nexus Mods Monitor (Unofficial)")
            .WithDescription(@$"**Version:** {Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "UNKNOWN"}
**Source: **[GitHub](https://www.google.com)

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

        public static Embed NewIssue(IssueDTO issue) => new EmbedBuilder()
            .WithTitle($"Bugs: [{issue.Title}] new report")
            .WithAuthor(issue.Content.Author, issue.Content.AvatarUrl, issue.Content.AuthorUrl)
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
            .WithTitle($"Bugs: [{issue.Title}] report was deleted")
            .WithAuthor(issue.Content.Author, issue.Content.AvatarUrl, issue.Content.AuthorUrl)
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

        public static Embed NewIssueReply(IssueDTO issue, IssueDTO.IssueReplyDTO issueReply) => new EmbedBuilder()
            .WithTitle($"Bugs: [{issue.Title}] new reply")
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

        public static Embed DeletedIssueReply(IssueDTO issueEntity, IssueDTO.IssueReplyDTO issueReply) => new EmbedBuilder()
            .WithTitle($"Bugs: [{issueEntity.Title}] reply was deleted")
            .WithDescription(issueEntity.Status.Name)
            .WithAuthor(issueReply.Author, issueReply.AvatarUrl, issueReply.AuthorUrl)
            .WithThumbnailUrl(issueReply.AvatarUrl)
            .WithCurrentTimestamp()
            .WithUrl(issueEntity.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Status")
                    .WithValue(issueEntity.Status.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issueEntity.Priority.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issueEntity.ModVersion)
                    .WithIsInline(true),

                new EmbedFieldBuilder()
                    .WithName("Private")
                    .WithValue(issueEntity.IsPrivate.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issueEntity.IsClosed.ToString())
                    .WithIsInline(true))
            .WithFields(new EmbedFieldBuilder()
                .WithName("Message")
                .WithValue(issueReply.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Embed StatusChanged(IssueDTO issueEntity, IssueDTO.IssueStatusDTO oldIssueStatus) => new EmbedBuilder()
            .WithTitle($"Bugs: [{issueEntity.Title}] status changed")
            .WithCurrentTimestamp()
            .WithDescription($"{oldIssueStatus.Name} -> {issueEntity.Status.Name}")
            .WithUrl(issueEntity.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Status")
                    .WithValue(issueEntity.Status.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issueEntity.Priority.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issueEntity.ModVersion)
                    .WithIsInline(true),

                new EmbedFieldBuilder()
                    .WithName("Private")
                    .WithValue(issueEntity.IsPrivate.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issueEntity.IsClosed.ToString())
                    .WithIsInline(true))
            .Build();

        public static Embed PriorityChanged(IssueDTO issueEntity, IssueDTO.IssuePriorityDTO oldIssuePriority) => new EmbedBuilder()
            .WithTitle($"Bugs: [{issueEntity.Title}] priority changed")
            .WithCurrentTimestamp()
            .WithDescription($"{oldIssuePriority.Name} -> {issueEntity.Priority.Name}")
            .WithUrl(issueEntity.Url)
            .WithColor(Color)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Status")
                    .WithValue(issueEntity.Status.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issueEntity.Priority.Name)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issueEntity.ModVersion)
                    .WithIsInline(true),

                new EmbedFieldBuilder()
                    .WithName("Private")
                    .WithValue(issueEntity.IsPrivate.ToString())
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issueEntity.IsClosed.ToString())
                    .WithIsInline(true))
            .Build();

        public static Embed IsClosedChanged(IssueDTO issue) => new EmbedBuilder()
            .WithTitle(issue.IsClosed ? $"Bugs: [{issue.Title}] is closed" : $"Bugs: [{issue.Title}] is re-opened")
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
            .WithTitle(issue.IsPrivate ? $"Bugs: [{issue.Title}] is made private" : $"Bugs: [{issue.Title}] is made public")
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
            .WithTitle($"Posts: [{comment.Id}] new post")
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
            .WithTitle($"Posts: [{comment.Id}] was deleted")
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

        public static Embed NewCommentReply(CommentDTO comment, CommentDTO.CommentReplyDTO commentReply) => new EmbedBuilder()
            .WithTitle($"Posts: [{comment.Id}] new reply")
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

        public static Embed DeletedCommentReply(CommentDTO comment, CommentDTO.CommentReplyDTO commentReply) => new EmbedBuilder()
            .WithTitle($"Posts: [{comment.Id}] reply was deleted")
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
            .WithTitle(comment.IsLocked ? $"Posts: [{comment.Id}] is locked" : $"Posts: [{comment.Id}] is unlocked")
            .WithCurrentTimestamp()
            .WithUrl(comment.Url)
            .WithColor(Color)
            .Build();

        public static Embed IsStickyChanged(CommentDTO comment) => new EmbedBuilder()
            .WithTitle(comment.IsSticky ? $"Posts: [{comment.Id}] is pinned" : $"Posts: [{comment.Id}] is unpinned")
            .WithCurrentTimestamp()
            .WithUrl(comment.Url)
            .WithColor(Color)
            .Build();

        private static string WithMaxLength(this string value, int maxLength) => value.Substring(0, Math.Min(value.Length, maxLength));
    }
}