namespace NexusMods.Monitor.Scraper.Application.Queries.Issues
{
    public sealed class IssueReplyViewModel
    {
        public uint Id { get; private set; } = default!;
        public uint OwnerId { get; private set; } = default!;

        private IssueReplyViewModel() { }
        public IssueReplyViewModel(uint id, uint ownerId)
        {
            Id = id;
            OwnerId = ownerId;
        }
    }
}