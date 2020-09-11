using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Subscriptions.Application.Commands
{
    [DataContract]
    public class SubscriptionRemoveCommand : IRequest<bool>
    {
        [DataMember]
        public string SubscriberId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsModId { get; private set; } = default!;

        private SubscriptionRemoveCommand() { }
        public SubscriptionRemoveCommand(string subscriberId, uint nexusModsGameId, uint nexusModsModId)
        {
            SubscriberId = subscriberId;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
        }
    }
}