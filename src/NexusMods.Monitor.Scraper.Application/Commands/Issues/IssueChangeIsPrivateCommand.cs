using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    [DataContract]
    public class IssueChangeIsPrivateCommand : IRequest<bool>
    {
        [DataMember]
        public uint Id { get; private set; }
        [DataMember]
        public bool IsPrivate { get; private set; }

        private IssueChangeIsPrivateCommand() { }
        public IssueChangeIsPrivateCommand(uint id, bool isPrivate) : this()
        {
            Id = id;
            IsPrivate = isPrivate;
        }
    }
}