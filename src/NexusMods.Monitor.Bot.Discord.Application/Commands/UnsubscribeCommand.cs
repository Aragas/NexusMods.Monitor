using MediatR;

namespace NexusMods.Monitor.Bot.Discord.Application.Commands
{
    public sealed record UnsubscribeCommand(ulong ChannelId, uint NexusModsGameId, uint NexusModsModId) : IRequest<bool>;
}