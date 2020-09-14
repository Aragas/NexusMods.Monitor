using MediatR;

using System.Runtime.Serialization;

namespace NexusMods.Monitor.Bot.Slack.Application.Commands
{
    [DataContract]
    public sealed class UnsubscribeCommand : IRequest<bool>
    {
        [DataMember]
        public string ChannelId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsModId { get; private set; } = default!;

        private UnsubscribeCommand() { }
        public UnsubscribeCommand(string channelId, uint nexusModsGameId, uint nexusModsModId) : this()
        {
            ChannelId = channelId;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
        }
    }
}