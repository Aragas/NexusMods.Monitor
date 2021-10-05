using MediatR;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues;
using NexusMods.Monitor.Shared.Common;

using NodaTime;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    public sealed record IssueAddNewReplyCommand(uint Id, uint ReplyId, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant TimeOfPost) : IRequest<bool>
    {
        public static IssueAddNewReplyCommand FromViewModel(NexusModsIssueRootViewModel nexusModsIssueRoot, NexusModsIssueReplyViewModel nexusModsIssueReply)
        {
            return new IssueAddNewReplyCommand(RecordUtils.Default<IssueAddNewReplyCommand>())
            {
                Id = nexusModsIssueRoot.NexusModsIssue.Id,
                ReplyId = nexusModsIssueReply.Id,
                Author = nexusModsIssueReply.Author,
                AuthorUrl = nexusModsIssueReply.AuthorUrl,
                AvatarUrl = nexusModsIssueReply.AvatarUrl,
                Content = nexusModsIssueReply.Content,
                TimeOfPost = nexusModsIssueReply.Time,
            };
        }
    }
}