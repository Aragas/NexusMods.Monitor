using MediatR;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues;

using NodaTime;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    [DataContract]
    public class IssueAddNewReplyCommand : IRequest<bool>
    {
        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public uint OwnerId { get; private set; } = default!;
        [DataMember]
        public string Author { get; private set; } = default!;
        [DataMember]
        public string AuthorUrl { get; private set; } = default!;
        [DataMember]
        public string AvatarUrl { get; private set; } = default!;
        [DataMember]
        public string Content { get; private set; } = default!;
        [DataMember]
        public bool IsDeleted { get; private set; } = default!;
        [DataMember]
        public Instant TimeOfPost { get; private set; } = default!;

        private IssueAddNewReplyCommand() { }
        public IssueAddNewReplyCommand(NexusModsIssueRootViewModel nexusModsIssueRoot, NexusModsIssueReplyViewModel nexusModsIssueReply) : this()
        {
            Id = nexusModsIssueReply.Id;
            OwnerId = nexusModsIssueRoot.NexusModsIssue.Id;
            Author = nexusModsIssueReply.Author;
            AuthorUrl = nexusModsIssueReply.AuthorUrl;
            AvatarUrl = nexusModsIssueReply.AvatarUrl;
            Content = nexusModsIssueReply.Content;
            IsDeleted = false;
            TimeOfPost = nexusModsIssueReply.Time;
        }
    }
}