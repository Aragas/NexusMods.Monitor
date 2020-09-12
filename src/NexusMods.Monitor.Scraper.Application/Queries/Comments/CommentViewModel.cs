using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Application.Queries.Comments
{
    public sealed class CommentViewModel
    {
        public uint Id { get; private set; } = default!;
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;
        public bool IsLocked { get; private set; } = default!;
        public bool IsSticky { get; private set; } = default!;

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