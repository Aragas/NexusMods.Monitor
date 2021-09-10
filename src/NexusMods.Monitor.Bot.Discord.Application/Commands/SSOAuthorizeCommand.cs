using MediatR;

using System;

namespace NexusMods.Monitor.Bot.Discord.Application.Commands
{
    public sealed record SSOAuthorizeCommand(Guid Id) : IRequest<bool>;
}