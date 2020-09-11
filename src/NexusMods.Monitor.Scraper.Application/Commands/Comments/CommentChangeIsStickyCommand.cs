using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    [DataContract]
    public class CommentChangeIsStickyCommand : IRequest<bool>
    {
        [DataMember]
        public uint Id { get; private set; }
        [DataMember]
        public bool IsSticky { get; private set; }

        private CommentChangeIsStickyCommand() { }
        public CommentChangeIsStickyCommand(uint id, bool isSticky) : this()
        {
            Id = id;
            IsSticky = isSticky;
        }
    }
}