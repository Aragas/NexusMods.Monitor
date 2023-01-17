using MediatR;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Common;

using NodaTime;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    public sealed record IssueAddCommand(uint Id, uint NexusModsGameId, uint NexusModsModId, string GameName, string ModName, string Title, string Url, string ModVersion, uint StatusId, uint PriorityId, bool IsPrivate, bool IsClosed, Instant TimeOfLastPost, IssueAddCommand.IssueContentDTO? Content, IReadOnlyList<IssueAddCommand.IssueReplyDTO> Replies) : IRequest<bool>
    {
        public sealed record IssueContentDTO(uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant TimeOfPost);

        public sealed record IssueReplyDTO(uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant TimeOfPost);

        public static IssueAddCommand FromViewModel(NexusModsIssueRootViewModel nexusModsIssueRoot, IssueStatusEnumeration issueStatus, IssuePriorityEnumeration issuePriority)
        {
            return new IssueAddCommand(RecordUtils.Default<IssueAddCommand>())
            {
                Id = nexusModsIssueRoot.Issue.Id,
                NexusModsGameId = nexusModsIssueRoot.GameId,
                NexusModsModId = nexusModsIssueRoot.ModId,
                GameName = nexusModsIssueRoot.GameName,
                ModName = nexusModsIssueRoot.ModName,
                Title = nexusModsIssueRoot.Issue.Title,
                Url = $"https://www.nexusmods.com/{nexusModsIssueRoot.GameDomain}/mods/{nexusModsIssueRoot.ModId}/?tab=bugs&issue_id={nexusModsIssueRoot.Issue.Id}",
                ModVersion = nexusModsIssueRoot.Issue.ModVersion,
                StatusId = issueStatus.Id,
                PriorityId = issuePriority.Id,
                IsPrivate = nexusModsIssueRoot.Issue.IsPrivate,
                IsClosed = nexusModsIssueRoot.Issue.IsClosed,
                TimeOfLastPost = nexusModsIssueRoot.Issue.LastPost,
                Content = nexusModsIssueRoot.IssueContent is null
                    ? null
                    : new IssueContentDTO(
                        nexusModsIssueRoot.IssueContent.Id,
                        nexusModsIssueRoot.IssueContent.Author,
                        nexusModsIssueRoot.IssueContent.AuthorUrl,
                        nexusModsIssueRoot.IssueContent.AvatarUrl,
                        nexusModsIssueRoot.IssueContent.Content,
                        nexusModsIssueRoot.IssueContent.Time
                    ),
                Replies = nexusModsIssueRoot.IssueReplies.Select(x => new IssueReplyDTO(
                    x.Id,
                    x.Author,
                    x.AuthorUrl,
                    x.AvatarUrl,
                    x.Content,
                    x.Time
                )).ToImmutableArray(),
            };
        }
    }
}