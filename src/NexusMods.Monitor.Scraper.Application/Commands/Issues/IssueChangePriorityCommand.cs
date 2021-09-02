using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    [DataContract]
    public sealed record IssueChangePriorityCommand : IRequest<bool>
    {
        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public int PriorityId { get; private set; } = default!;

        private IssueChangePriorityCommand() { }
        public IssueChangePriorityCommand(uint id, int priorityId) : this()
        {
            Id = id;
            PriorityId = priorityId;
        }
    }
}