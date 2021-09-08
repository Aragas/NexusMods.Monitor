using MediatR;

namespace NexusMods.Monitor.Bot.Discord.Application.Commands
{
    public sealed record Unsubscribe2Command(ulong ChannelId, string NexusModsUrl) : IRequest<bool>;
}