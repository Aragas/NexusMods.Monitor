using MediatR;

namespace NexusMods.Monitor.Bot.Slack.Application.Commands
{
    public sealed record Subscribe2Command(string ChannelId, string NexusModsUrl) : IRequest<bool>;
}