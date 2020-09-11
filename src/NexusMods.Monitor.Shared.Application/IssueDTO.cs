using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Shared.Application
{
    public class IssueDTO
    {
        public uint Id { get; set; } = default!;
        public uint NexusModsGameId { get; set; } = default!;
        public uint NexusModsModId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string ModVersion { get; set; } = default!;
        public IssueStatusDTO Status { get; set; } = default!;
        public IssuePriorityDTO Priority { get; set; } = default!;
        public bool IsPrivate { get; set; } = default!;
        public bool IsClosed { get; set; } = default!;
        public bool IsDeleted { get; set; } = default!;
        public DateTimeOffset TimeOfLastPost { get; set; } = default!;
        public IssueContentDTO? Content { get; set; } = default!;
        public List<IssueReplyDTO> Replies { get; set; } = new List<IssueReplyDTO>();

        public class IssueStatusDTO
        {
            public uint Id { get; set; } = default!;
            public string Name { get; set; } = default!;
        }

        public class IssuePriorityDTO
        {
            public uint Id { get; set; } = default!;
            public string Name { get; set; } = default!;
        }

        public class IssueContentDTO
        {
            public uint Id { get; set; } = default!;
            public string Author { get; set; } = default!;
            public string AuthorUrl { get; set; } = default!;
            public string AvatarUrl { get; set; } = default!;
            public string Content { get; set; } = default!;
            public bool IsDeleted { get; set; } = default!;
            public DateTimeOffset TimeOfPost { get; set; } = default!;
        }

        public class IssueReplyDTO
        {
            public uint Id { get; set; } = default!;
            public string Author { get; set; } = default!;
            public string AuthorUrl { get; set; } = default!;
            public string AvatarUrl { get; set; } = default!;
            public string Content { get; set; } = default!;
            public bool IsDeleted { get; set; } = default!;
            public DateTimeOffset TimeOfPost { get; set; } = default!;
        }
    }
}
