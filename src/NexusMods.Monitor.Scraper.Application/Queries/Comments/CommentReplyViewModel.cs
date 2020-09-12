namespace NexusMods.Monitor.Scraper.Application.Queries.Comments
{
    public sealed class CommentReplyViewModel
    {
        public uint Id { get; private set; } = default!;
        public uint OwnerId { get; private set; } = default!;

        private CommentReplyViewModel() { }
        public CommentReplyViewModel(uint id, uint ownerId)
        {
            Id = id;
            OwnerId = ownerId;
        }
    }
}