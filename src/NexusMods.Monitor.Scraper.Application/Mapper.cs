using NexusMods.Monitor.Scraper.Application.Commands.Comments;
using NexusMods.Monitor.Scraper.Application.Commands.Issues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Application.Models;

using NodaTime;

using System;
using System.Collections.Immutable;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Application
{
    public static class Mapper
    {
        public static DateTimeOffset Map(Instant x) => x.ToDateTimeOffset();

        public static CommentDTO Map(CommentEntity x) => new(
            x.Id,
            x.NexusModsGameId,
            x.NexusModsModId,
            x.GameName,
            x.ModName,
            x.Url,
            x.Author,
            x.AuthorUrl,
            x.AvatarUrl,
            x.Content,
            x.IsSticky,
            x.IsLocked,
            Map(x.TimeOfPost),
            x.Replies.Select(Map).ToImmutableArray()
        );

        public static CommentReplyDTO Map(CommentReplyEntity x) => new(x.Id, x.Url, x.Author, x.AuthorUrl, x.AvatarUrl, x.Content, x.IsDeleted, Map(x.TimeOfPost));

        public static CommentEntity Map(CommentAddCommand x)
        {
            var commentEntity = new CommentEntity(
                x.Id,
                x.NexusModsGameId,
                x.NexusModsModId,
                x.GameName,
                x.ModName,
                x.Url,
                x.Author,
                x.AuthorUrl,
                x.AvatarUrl,
                x.Content,
                x.IsSticky,
                x.IsLocked,
                false,
                x.TimeOfPost);

            foreach (var commentReply in x.Replies)
            {
                commentEntity.AddReplyEntity(
                    commentReply.Id,
                    commentReply.Url,
                    commentReply.Author,
                    commentReply.AuthorUrl,
                    commentReply.AvatarUrl,
                    commentReply.Content,
                    false,
                    commentReply.TimeOfPost);
            }

            return commentEntity;
        }

        public static CommentEntity Map(CommentAddNewCommand x)
        {
            var commentEntity = new CommentEntity(
                x.Id,
                x.NexusModsGameId,
                x.NexusModsModId,
                x.GameName,
                x.ModName,
                x.Url,
                x.Author,
                x.AuthorUrl,
                x.AvatarUrl,
                x.Content,
                x.IsSticky,
                x.IsLocked,
                false,
                x.TimeOfPost);

            foreach (var commentReply in x.Replies)
            {
                commentEntity.AddReplyEntity(
                    commentReply.Id,
                    commentReply.Url,
                    commentReply.Author,
                    commentReply.AuthorUrl,
                    commentReply.AvatarUrl,
                    commentReply.Content,
                    false,
                    commentReply.TimeOfPost);
            }

            return commentEntity;
        }


        public static IssueEntity Map(IssueAddCommand x, IssueStatusEnumeration s, IssuePriorityEnumeration p)
        {
            var issueEntity = new IssueEntity(
                x.Id,
                x.NexusModsGameId,
                x.NexusModsModId,
                x.GameName,
                x.ModName,
                x.Title,
                x.Url,
                x.ModVersion,
                s,
                p,
                x.IsPrivate,
                x.IsClosed,
                false,
                x.TimeOfLastPost);

            if (x.Content is { })
            {
                issueEntity.SetContent(
                    x.Content.Author,
                    x.Content.AuthorUrl,
                    x.Content.AvatarUrl,
                    x.Content.Content,
                    false,
                    x.Content.TimeOfPost);
            }

            foreach (var issueReply in x.Replies)
            {
                issueEntity.AddReplyEntity(
                    issueReply.Id,
                    issueReply.Author,
                    issueReply.AuthorUrl,
                    issueReply.AvatarUrl,
                    issueReply.Content,
                    false,
                    issueReply.TimeOfPost);
            }

            return issueEntity;
        }

        public static IssueEntity Map(IssueAddNewCommand x, IssueStatusEnumeration s, IssuePriorityEnumeration p)
        {
            var issueEntity = new IssueEntity(
                x.Id,
                x.NexusModsGameId,
                x.NexusModsModId,
                x.GameName,
                x.ModName,
                x.Title,
                x.Url,
                x.ModVersion,
                s,
                p,
                x.IsPrivate,
                x.IsClosed,
                false,
                x.TimeOfLastPost);

            if (x.Content is { })
            {
                issueEntity.SetContent(
                    x.Content.Author,
                    x.Content.AuthorUrl,
                    x.Content.AvatarUrl,
                    x.Content.Content,
                    false,
                    x.Content.TimeOfPost);
            }

            foreach (var issueReply in x.Replies)
            {
                issueEntity.AddReplyEntity(
                    issueReply.Id,
                    issueReply.Author,
                    issueReply.AuthorUrl,
                    issueReply.AvatarUrl,
                    issueReply.Content,
                    false,
                    issueReply.TimeOfPost);
            }

            return issueEntity;
        }

        public static IssueDTO Map(IssueEntity x) => new(
            x.Id,
            x.NexusModsGameId,
            x.NexusModsModId,
            x.GameName,
            x.ModName,
            x.Title,
            x.Url,
            x.ModVersion,
            Map(x.Status),
            Map(x.Priority),
            x.IsPrivate,
            x.IsClosed,
            Map(x.TimeOfLastPost),
            x.Content is null ? null : Map(x.Content),
            x.Replies.Select(Map).ToImmutableArray()
        );

        public static IssueStatusDTO Map(IssueStatusEnumeration x) => new(x.Id, x.Name);

        public static IssuePriorityDTO Map(IssuePriorityEnumeration x) => new(x.Id, x.Name);

        public static IssueContentDTO Map(IssueContentEntity x) => new(x.Id, x.Author, x.AuthorUrl, x.AvatarUrl, x.Content, Map(x.TimeOfPost));

        public static IssueReplyDTO Map(IssueReplyEntity x) => new(x.Id, x.Author, x.AuthorUrl, x.AvatarUrl, x.Content, Map(x.TimeOfPost));
    }
}