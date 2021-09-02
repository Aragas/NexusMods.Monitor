using MediatR;

namespace NexusMods.Monitor.Subscriptions.Application.Commands
{
    public sealed record SubscriptionRemoveCommand(string SubscriberId, uint NexusModsGameId, uint NexusModsModId) : IRequest<bool>;
}