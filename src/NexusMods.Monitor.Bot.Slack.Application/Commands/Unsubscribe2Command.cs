using MediatR;

namespace NexusMods.Monitor.Bot.Slack.Application.Commands
{
    public sealed record Unsubscribe2Command(string ChannelId, string NexusModsUrl) : IRequest<bool>;
}