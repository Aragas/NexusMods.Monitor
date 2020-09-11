using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Comments
{
    [DataContract]
    public class CommentRemoveCommand : IRequest<bool>
    {
        [DataMember]
        public uint Id { get; private set; }

        private CommentRemoveCommand() { }
        public CommentRemoveCommand(uint id) : this()
        {
            Id = id;
        }
    }
}