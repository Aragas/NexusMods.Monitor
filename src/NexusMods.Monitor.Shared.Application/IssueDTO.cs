using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusMods.Monitor.Shared.Application
{
    public sealed class IssueDTO
    {
        public uint Id { get; private set; } = default!;
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;
        public string Title { get; private set; } = default!;
        public string Url { get; private set; } = default!;
        public string ModVersion { get; private set; } = default!;
        public IssueStatusDTO Status { get; private set; } = default!;
        public IssuePriorityDTO Priority { get; private set; } = default!;
        public bool IsPrivate { get; private set; } = default!;
        public bool IsClosed { get; private set; } = default!;
        public DateTimeOffset TimeOfLastPost { get; private set; } = default!;
        public IssueContentDTO? Content { get; private set; } = default!;
        public IReadOnlyCollection<IssueReplyDTO> Replies { get; private set; } = default!;

        private IssueDTO()
        {
            Replies = new List<IssueReplyDTO>();
        }
        public IssueDTO(uint id, uint nexusModsGameId, uint nexusModsModId, string title, string url, string modVersion, IssueStatusDTO status, IssuePriorityDTO priority, bool isPrivate, bool isClosed, DateTimeOffset timeOfLastPost, IEnumerable<IssueReplyDTO> replies) : this()
        {
            Id = id;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            Title = title;
            Url = url;
            ModVersion = modVersion;
            Status = status;
            Priority = priority;
            IsPrivate = isPrivate;
            IsClosed = isClosed;
            TimeOfLastPost = timeOfLastPost;
            Replies = replies.ToList();
        }
    }

    public sealed class IssueStatusDTO
    {
        public uint Id { get; private set; } = default!;
        public string Name { get; private set; } = default!;

        private IssueStatusDTO() { }
        public IssueStatusDTO(uint id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public sealed class IssuePriorityDTO
    {
        public uint Id { get; private set; } = default!;
        public string Name { get; private set; } = default!;

        private IssuePriorityDTO() { }
        public IssuePriorityDTO(uint id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public sealed class IssueContentDTO
    {
        public uint Id { get; private set; } = default!;
        public string Author { get; private set; } = default!;
        public string AuthorUrl { get; private set; } = default!;
        public string AvatarUrl { get; private set; } = default!;
        public string Content { get; private set; } = default!;
        public DateTimeOffset TimeOfPost { get; private set; } = default!;

        private IssueContentDTO() { }
        public IssueContentDTO(uint id, string author, string authorUrl, string avatarUrl, string content, DateTimeOffset timeOfPost)
        {
            Id = id;
            Author = author;
            AuthorUrl = authorUrl;
            AvatarUrl = avatarUrl;
            Content = content;
            TimeOfPost = timeOfPost;
        }
    }

    public sealed class IssueReplyDTO
    {
        public uint Id { get; private set; } = default!;
        public string Author { get; private set; } = default!;
        public string AuthorUrl { get; private set; } = default!;
        public string AvatarUrl { get; private set; } = default!;
        public string Content { get; private set; } = default!;
        public DateTimeOffset TimeOfPost { get; private set; } = default!;

        private IssueReplyDTO() { }
        public IssueReplyDTO(uint id, string author, string authorUrl, string avatarUrl, string content, DateTimeOffset timeOfPost)
        {
            Id = id;
            Author = author;
            AuthorUrl = authorUrl;
            AvatarUrl = avatarUrl;
            Content = content;
            TimeOfPost = timeOfPost;
        }
    }
}