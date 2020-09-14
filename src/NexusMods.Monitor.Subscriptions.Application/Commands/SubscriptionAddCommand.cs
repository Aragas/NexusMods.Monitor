using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Subscriptions.Application.Commands
{
    [DataContract]
    public sealed class SubscriptionAddCommand : IRequest<bool>
    {
        [DataMember]
        public string SubscriberId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsModId { get; private set; } = default!;

        private SubscriptionAddCommand() { }
        public SubscriptionAddCommand(string subscriberId, uint nexusModsGameId, uint nexusModsModId)
        {
            SubscriberId = subscriberId;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
        }
    }
}