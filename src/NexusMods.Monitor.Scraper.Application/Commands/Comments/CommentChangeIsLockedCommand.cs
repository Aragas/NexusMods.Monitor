using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    [DataContract]
    public class CommentChangeIsLockedCommand : IRequest<bool>
    {
        [DataMember]
        public uint Id { get; private set; }
        [DataMember]
        public bool IsLocked { get; private set; }

        private CommentChangeIsLockedCommand() { }
        public CommentChangeIsLockedCommand(uint id, bool isLocked) : this()
        {
            Id = id;
            IsLocked = isLocked;
        }
    }
}