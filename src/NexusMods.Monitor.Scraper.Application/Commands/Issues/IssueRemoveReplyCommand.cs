using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    [DataContract]
    public class IssueRemoveReplyCommand : IRequest<bool>
    {
        [DataMember]
        public uint Id { get; private set; }
        [DataMember]
        public uint ReplyId { get; private set; }

        private IssueRemoveReplyCommand() { }
        public IssueRemoveReplyCommand(uint id, uint replyId) : this()
        {
            Id = id;
            ReplyId = replyId;
        }
    }
}