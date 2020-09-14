using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Queries.Comments
{
    [DataContract]
    public sealed class CommentViewModel
    {
        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsModId { get; private set; } = default!;
        [DataMember]
        public bool IsLocked { get; private set; } = default!;
        [DataMember]
        public bool IsSticky { get; private set; } = default!;

        [DataMember]
        private readonly List<CommentReplyViewModel> _replies;
        public IEnumerable<CommentReplyViewModel> Replies => _replies;

        private CommentViewModel()
        {
            _replies = new List<CommentReplyViewModel>();
        }
        public CommentViewModel(uint id, uint nexusModsGameId, uint nexusModsModId, bool isLocked, bool isSticky, IEnumerable<CommentReplyViewModel> commentReplies) : this()
        {
            Id = id;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            IsLocked = isLocked;
            IsSticky = isSticky;
            _replies.AddRange(commentReplies);
        }
    }
}