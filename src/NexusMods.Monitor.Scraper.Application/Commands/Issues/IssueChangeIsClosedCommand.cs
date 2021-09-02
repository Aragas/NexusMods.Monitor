using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    [DataContract]
    public sealed record IssueChangeIsClosedCommand : IRequest<bool>
    {
        [DataMember]
        public uint Id { get; private set; }
        [DataMember]
        public bool IsClosed { get; private set; }

        private IssueChangeIsClosedCommand() { }
        public IssueChangeIsClosedCommand(uint id, bool isClosed) : this()
        {
            Id = id;
            IsClosed = isClosed;
        }
    }
}