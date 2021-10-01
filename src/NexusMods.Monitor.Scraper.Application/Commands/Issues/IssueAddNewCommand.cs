using MediatR;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Common;

using NodaTime;

using System.Collections.Immutable;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    public sealed record IssueAddNewCommand(uint Id, uint NexusModsGameId, uint NexusModsModId, string GameName, string ModName, string Title, string Url, string ModVersion, IssueStatusEnumeration Status, IssuePriorityEnumeration Priority, bool IsPrivate, bool IsClosed, Instant TimeOfLastPost, IssueAddNewCommand.IssueContentDTO? Content, ImmutableArray<IssueAddNewCommand.IssueReplyDTO> Replies) : IRequest<bool>
    {
        public sealed record IssueContentDTO(uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant TimeOfPost);

        public sealed record IssueReplyDTO(uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant TimeOfPost);

        public static IssueAddNewCommand FromViewModel(NexusModsIssueRootViewModel nexusModsIssueRoot, IssueStatusEnumeration issueStatus, IssuePriorityEnumeration issuePriority)
        {
            return new IssueAddNewCommand(RecordUtils.Default<IssueAddNewCommand>())
            {
                Id = nexusModsIssueRoot.NexusModsIssue.Id,
                NexusModsGameId = nexusModsIssueRoot.GameId,
                NexusModsModId = nexusModsIssueRoot.ModId,
                GameName = nexusModsIssueRoot.GameName,
                ModName = nexusModsIssueRoot.ModName,
                Title = nexusModsIssueRoot.NexusModsIssue.Title,
                Url = $"https://www.nexusmods.com/{nexusModsIssueRoot.GameDomain}/mods/{nexusModsIssueRoot.ModId}/?tab=bugs&issue_id={nexusModsIssueRoot.NexusModsIssue.Id}",
                ModVersion = nexusModsIssueRoot.NexusModsIssue.ModVersion,
                Status = issueStatus,
                Priority = issuePriority,
                IsPrivate = nexusModsIssueRoot.NexusModsIssue.IsPrivate,
                IsClosed = nexusModsIssueRoot.NexusModsIssue.IsClosed,
                TimeOfLastPost = nexusModsIssueRoot.NexusModsIssue.LastPost,
                Content = nexusModsIssueRoot.NexusModsIssueContent is null
                    ? null
                    : new IssueContentDTO(
                        nexusModsIssueRoot.NexusModsIssueContent.Id,
                        nexusModsIssueRoot.NexusModsIssueContent.Author,
                        nexusModsIssueRoot.NexusModsIssueContent.AuthorUrl,
                        nexusModsIssueRoot.NexusModsIssueContent.AvatarUrl,
                        nexusModsIssueRoot.NexusModsIssueContent.Content,
                        nexusModsIssueRoot.NexusModsIssueContent.Time
                    ),
                Replies = nexusModsIssueRoot.NexusModsIssueReplies.Select(x => new IssueReplyDTO(
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