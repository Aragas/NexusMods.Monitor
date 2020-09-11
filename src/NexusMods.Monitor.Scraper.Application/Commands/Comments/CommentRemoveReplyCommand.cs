using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    [DataContract]
    public class CommentRemoveReplyCommand : IRequest<bool>
    {
        [DataMember]
        public uint Id { get; private set; }
        [DataMember]
        public uint ReplyId { get; private set; }

        private CommentRemoveReplyCommand() { }
        public CommentRemoveReplyCommand(uint id, uint replyId) : this()
        {
            Id = id;
            ReplyId = replyId;
        }
    }
}