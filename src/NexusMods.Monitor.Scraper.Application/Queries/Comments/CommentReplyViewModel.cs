using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Queries.Comments
{
    [DataContract]
    public sealed class CommentReplyViewModel
    {
        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public uint OwnerId { get; private set; } = default!;

        private CommentReplyViewModel() { }
        public CommentReplyViewModel(uint id, uint ownerId) : this()
        {
            Id = id;
            OwnerId = ownerId;
        }
    }
}