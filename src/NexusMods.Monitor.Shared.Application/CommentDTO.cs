using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Shared.Application
{
    public class CommentDTO
    {
        public uint Id { get; set;} = default!;
        public uint NexusModsGameId { get; set;} = default!;
        public uint NexusModsModId { get; set;} = default!;
        public string Url { get; set;} = default!;
        public string Author { get;set; } = default!;
        public string AuthorUrl { get; set;} = default!;
        public string AvatarUrl { get; set;} = default!;
        public string Content { get; set;} = default!;
        public bool IsSticky { get;set; } = default!;
        public bool IsLocked { get;set; } = default!;
        public DateTimeOffset TimeOfPost { get; set;} = default!;
        public List<CommentReplyDTO> Replies  { get; set; } = new List<CommentReplyDTO>();

        public class CommentReplyDTO
        {
            public uint Id { get; set; } = default!;
            public string Url { get; set; } = default!;
            public string Author { get; set; } = default!;
            public string AuthorUrl { get; set; } = default!;
            public string AvatarUrl { get; set; } = default!;
            public string Content { get; set; } = default!;
            public DateTimeOffset TimeOfPost { get; set; } = default!;
        }
    }
}