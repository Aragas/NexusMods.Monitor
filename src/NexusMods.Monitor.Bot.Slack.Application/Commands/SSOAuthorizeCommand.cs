using MediatR;

using System;

namespace NexusMods.Monitor.Bot.Slack.Application.Commands
{
    public sealed record SSOAuthorizeCommand(Guid Id) : IRequest<bool>;
}