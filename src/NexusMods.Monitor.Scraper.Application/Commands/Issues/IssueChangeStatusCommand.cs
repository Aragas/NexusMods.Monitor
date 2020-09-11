﻿using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Commands.Issues
{
    [DataContract]
    public class IssueChangeStatusCommand : IRequest<bool>
    {
        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public int StatusId { get; private set; } = default!;

        private IssueChangeStatusCommand() { }
        public IssueChangeStatusCommand(uint id, int statusId) : this()
        {
            Id = id;
            StatusId = statusId;
        }
    }
}