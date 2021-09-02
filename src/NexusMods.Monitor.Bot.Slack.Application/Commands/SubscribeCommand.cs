using MediatR;

namespace NexusMods.Monitor.Bot.Slack.Application.Commands
{
    public sealed record SubscribeCommand(string ChannelId, uint NexusModsGameId, uint NexusModsModId) : IRequest<bool>;
}