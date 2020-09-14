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
    public sealed class IssueAddCommand : IRequest<bool>
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

        private IssueAddCommand()
        {
            _replies = new List<IssueReplyDTO>();
        }
        public IssueAddCommand(NexusModsIssueRootViewModel nexusModsIssueRoot) : this()
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
                Content = new IssueContentDTO(
                    nexusModsIssueRoot.NexusModsIssueContent.Id,
                    nexusModsIssueRoot.NexusModsIssueContent.Author,
                    nexusModsIssueRoot.NexusModsIssueContent.AuthorUrl,
                    nexusModsIssueRoot.NexusModsIssueContent.AvatarUrl,
                    nexusModsIssueRoot.NexusModsIssueContent.Content,
                    false,
                    nexusModsIssueRoot.NexusModsIssueContent.Time
                );
            }

            if (!(nexusModsIssueRoot.NexusModsIssueReplies is null))
            {
                _replies = nexusModsIssueRoot.NexusModsIssueReplies.Select(x => new IssueReplyDTO(
                    x.Id,
                    x.Author,
                    x.AuthorUrl,
                    x.AvatarUrl,
                    x.Content,
                    false,
                    x.Time
                )).ToList();
            }
        }

        [DataContract]
        public sealed class IssueContentDTO
        {
            [DataMember]
            public uint Id { get; private set; } = default!;
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

            private IssueContentDTO() { }
            public IssueContentDTO(uint id, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant time) : this()
            {
                Id = id;
                Author = author;
                AuthorUrl = authorUrl;
                AvatarUrl = avatarUrl;
                Content = content;
                IsDeleted = isDeleted;
                TimeOfPost = time;
            }
        }

        [DataContract]
        public sealed class IssueReplyDTO
        {
            [DataMember]
            public uint Id { get; private set; } = default!;
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

            private IssueReplyDTO() { }
            public IssueReplyDTO(uint id, string author, string authorUrl, string avatarUrl, string content, bool isDeleted, Instant time) : this()
            {
                Id = id;
                Author = author;
                AuthorUrl = authorUrl;
                AvatarUrl = avatarUrl;
                Content = content;
                IsDeleted = isDeleted;
                TimeOfPost = time;
            }
        }
    }
}