using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Bot.Discord.Application.Commands
{
    [DataContract]
    public class SubscribeCommand : IRequest<bool>
    {
        [DataMember]
        public ulong ChannelId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsModId { get; private set; } = default!;

        private SubscribeCommand() { }
        public SubscribeCommand(ulong channelId, uint nexusModsGameId, uint nexusModsModId) : this()
        {
            ChannelId = channelId;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
        }
    }
}