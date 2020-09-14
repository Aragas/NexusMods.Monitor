using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Queries.Issues
{
    [DataContract]
    public sealed class IssueReplyViewModel
    {
        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public uint OwnerId { get; private set; } = default!;

        private IssueReplyViewModel() { }
        public IssueReplyViewModel(uint id, uint ownerId) : this()
        {
            Id = id;
            OwnerId = ownerId;
        }
    }
}