using MediatR;

using NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;

using NodaTime;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    [DataContract]
    public class IssueAddNewCommand : IRequest<bool>
    {
        [DataMember]
        private readonly List<IssueReplyDTO> _replies = default!;

        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsModId { get; private set; } = default!;
        [DataMember]
        public string Title { get; private set; } = default!;
        [DataMember]
        public string Url { get; private set; } = default!;
        [DataMember]
        public string ModVersion { get; private set; } = default!;
        [DataMember]
        public IssueStatusEnumeration Status { get; private set; } = default!;
        [DataMember]
        public IssuePriorityEnumeration Priority { get; private set; } = default!;
        [DataMember]
        public bool IsPrivate { get; private set; } = default!;
        [DataMember]
        public bool IsClosed { get; private set; } = default!;
        [DataMember]
        public bool IsDeleted { get; private set; } = default!;
        [DataMember]
        public Instant TimeOfLastPost { get; private set; } = default!;
        [DataMember]
        public IssueContentDTO? Content { get; private set; } = default!;
        [DataMember]
        public IEnumerable<IssueReplyDTO> Replies => _replies;

        private IssueAddNewCommand()
        {
            _replies = new List<IssueReplyDTO>();
        }
        public IssueAddNewCommand(NexusModsIssueRootViewModel nexusModsIssueRoot) : this()
        {
            Id = nexusModsIssueRoot.NexusModsIssue.Id;
            NexusModsGameId = nexusModsIssueRoot.NexusModsGameId;
            NexusModsModId = nexusModsIssueRoot.NexusModsModId;
            Title = nexusModsIssueRoot.NexusModsIssue.Title;
            Url = $"https://www.nexusmods.com/{nexusModsIssueRoot.NexusModsGameIdText}/mods/{NexusModsModId}/?tab=bugs&issue_id={Id}";
            ModVersion = nexusModsIssueRoot.NexusModsIssue.ModVersion;
            Status = nexusModsIssueRoot.NexusModsIssue.Status;
            Priority = nexusModsIssueRoot.NexusModsIssue.Priority;
            IsPrivate = nexusModsIssueRoot.NexusModsIssue.IsPrivate;
            IsClosed = nexusModsIssueRoot.NexusModsIssue.IsClosed;
            IsDeleted = false;
            TimeOfLastPost = nexusModsIssueRoot.NexusModsIssue.LastPost;

            if (!(nexusModsIssueRoot.NexusModsIssueContent is null))
            {
                Content = new IssueContentDTO
                {
                    Id = nexusModsIssueRoot.NexusModsIssueContent.Id,
                    Author = nexusModsIssueRoot.NexusModsIssueContent.Author,
                    AuthorUrl = nexusModsIssueRoot.NexusModsIssueContent.AuthorUrl,
                    AvatarUrl = nexusModsIssueRoot.NexusModsIssueContent.AvatarUrl,
                    Content = nexusModsIssueRoot.NexusModsIssueContent.Content,
                    IsDeleted = false,
                    TimeOfPost = nexusModsIssueRoot.NexusModsIssueContent.Time
                };
            }

            if (!(nexusModsIssueRoot.NexusModsIssueReplies is null))
            {
                _replies = nexusModsIssueRoot.NexusModsIssueReplies.Select(x => new IssueReplyDTO
                {
                    Id = x.Id,
                    Author = x.Author,
                    AuthorUrl = x.AuthorUrl,
                    AvatarUrl = x.AvatarUrl,
                    Content = x.Content,
                    IsDeleted = false,
                    TimeOfPost = x.Time
                }).ToList();
            }
        }

        public class IssueContentDTO
        {
            public uint Id { get; set; } = default!;
            public string Author { get; set; } = default!;
            public string AuthorUrl { get; set; } = default!;
            public string AvatarUrl { get; set; } = default!;
            public string Content { get; set; } = default!;
            public bool IsDeleted { get; set; } = default!;
            public Instant TimeOfPost { get; set; } = default!;
        }

        public class IssueReplyDTO
        {
            public uint Id { get; set; } = default!;
            public string Author { get; set; } = default!;
            public string AuthorUrl { get; set; } = default!;
            public string AvatarUrl { get; set; } = default!;
            public string Content { get; set; } = default!;
            public bool IsDeleted { get; set; } = default!;
            public Instant TimeOfPost { get; set; } = default!;
        }
    }
}