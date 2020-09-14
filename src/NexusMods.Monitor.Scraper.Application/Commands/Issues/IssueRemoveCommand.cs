using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    [DataContract]
    public sealed class IssueRemoveCommand : IRequest<bool>
    {
        [DataMember]
        public uint Id { get; private set; }

        private IssueRemoveCommand() { }
        public IssueRemoveCommand(uint id) : this()
        {
            Id = id;
        }
    }
}