using MediatR;

using NexusMods.Monitor.Shared.Application.SSE;

using System;

namespace NexusMods.Monitor.Bot.Discord.Application.Commands
{
    public sealed record SSOAuthorizeCommand(Guid Id) : IRequest<ISSOAuthorizationHandler>;
}