using MediatR;

namespace NexusMods.Monitor.Bot.Discord.Application.Commands
{
    public sealed record SubscribeCommand(ulong ChannelId, uint NexusModsGameId, uint NexusModsModId) : IRequest<bool>;
}