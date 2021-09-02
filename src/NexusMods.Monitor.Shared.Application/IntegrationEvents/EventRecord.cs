using Enbiso.NLib.EventBus;

using System;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents
{
    public abstract record EventRecord(Guid EventId, DateTime EventCreationDate) : IEvent
    {
        public EventRecord() : this(Guid.NewGuid(), DateTime.UtcNow)
        {
            EventId = Guid.NewGuid();
            EventCreationDate = DateTime.UtcNow;
        }
    }
}