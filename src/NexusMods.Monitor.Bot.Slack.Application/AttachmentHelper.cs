using NexusMods.Monitor.Shared.Application;

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

        public static Attachment NewIssue(IssueDTO issue) => new AttachmentBuilder()
            .WithTitle($"Bugs: [{issue.Title}] new report")
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
            .WithTitle($"Bugs: [{issue.Title}] report was deleted")
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
            .WithTitle($"Bugs: [{issue.Title}] new reply")
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

        public static Attachment DeletedIssueReply(IssueDTO issueEntity, IssueReplyDTO issueReply) => new AttachmentBuilder()
            .WithTitle($"Bugs: [{issueEntity.Title}] reply was deleted")
            .WithDescription(issueEntity.Status.Name)
            .WithAuthor(issueReply.Author, issueReply.AvatarUrl, issueReply.AuthorUrl)
            .WithThumbnailUrl(issueReply.AvatarUrl)
            .WithCurrentTimestamp()
            .WithUrl(issueEntity.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Status")
                    .WithValue(issueEntity.Status.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issueEntity.Priority.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issueEntity.ModVersion)
                    .WithIsInline(true),

                new AttachmentFieldBuilder()
                    .WithName("Private")
                    .WithValue(issueEntity.IsPrivate.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issueEntity.IsClosed.ToString())
                    .WithIsInline(true))
            .WithFields(new AttachmentFieldBuilder()
                .WithName("Message")
                .WithValue(issueReply.Content.WithMaxLength(MaxTextLength)))
            .Build();

        public static Attachment StatusChanged(IssueDTO issueEntity, IssueStatusDTO oldIssueStatus) => new AttachmentBuilder()
            .WithTitle($"Bugs: [{issueEntity.Title}] status changed")
            .WithCurrentTimestamp()
            .WithDescription($"{oldIssueStatus.Name} -> {issueEntity.Status.Name}")
            .WithUrl(issueEntity.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Status")
                    .WithValue(issueEntity.Status.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issueEntity.Priority.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issueEntity.ModVersion)
                    .WithIsInline(true),

                new AttachmentFieldBuilder()
                    .WithName("Private")
                    .WithValue(issueEntity.IsPrivate.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issueEntity.IsClosed.ToString())
                    .WithIsInline(true))
            .Build();

        public static Attachment PriorityChanged(IssueDTO issueEntity, IssuePriorityDTO oldIssuePriority) => new AttachmentBuilder()
            .WithTitle($"Bugs: [{issueEntity.Title}] priority changed")
            .WithCurrentTimestamp()
            .WithDescription($"{oldIssuePriority.Name} -> {issueEntity.Priority.Name}")
            .WithUrl(issueEntity.Url)
            .WithColor(Color)
            .WithFields(
                new AttachmentFieldBuilder()
                    .WithName("Status")
                    .WithValue(issueEntity.Status.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Priority")
                    .WithValue(issueEntity.Priority.Name)
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Mod Version")
                    .WithValue(issueEntity.ModVersion)
                    .WithIsInline(true),

                new AttachmentFieldBuilder()
                    .WithName("Private")
                    .WithValue(issueEntity.IsPrivate.ToString())
                    .WithIsInline(true),
                new AttachmentFieldBuilder()
                    .WithName("Closed")
                    .WithValue(issueEntity.IsClosed.ToString())
                    .WithIsInline(true))
            .Build();

        public static Attachment IsClosedChanged(IssueDTO issue) => new AttachmentBuilder()
            .WithTitle(issue.IsClosed ? $"Bugs: [{issue.Title}] is closed" : $"Bugs: [{issue.Title}] is re-opened")
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
            .WithTitle(issue.IsPrivate ? $"Bugs: [{issue.Title}] is made private" : $"Bugs: [{issue.Title}] is made public")
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
            .WithTitle($"Posts: [{comment.Id}] new post")
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
            .WithTitle($"Posts: [{comment.Id}] was deleted")
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
            .WithTitle($"Posts: [{comment.Id}] new reply")
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
            .WithTitle($"Posts: [{comment.Id}] reply was deleted")
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
            .WithTitle(comment.IsLocked ? $"Posts: [{comment.Id}] is locked" : $"Posts: [{comment.Id}] is unlocked")
            .WithCurrentTimestamp()
            .WithUrl(comment.Url)
            .WithColor(Color)
            .Build();

        public static Attachment IsStickyChanged(CommentDTO comment) => new AttachmentBuilder()
            .WithTitle(comment.IsSticky ? $"Posts: [{comment.Id}] is pinned" : $"Posts: [{comment.Id}] is unpinned")
            .WithCurrentTimestamp()
            .WithUrl(comment.Url)
            .WithColor(Color)
            .Build();

        private static string WithMaxLength(this string value, int maxLength) => value.Substring(0, Math.Min(value.Length, maxLength));
    }
}