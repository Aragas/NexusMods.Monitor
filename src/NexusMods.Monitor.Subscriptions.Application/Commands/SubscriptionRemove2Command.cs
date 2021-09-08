using MediatR;

namespace NexusMods.Monitor.Subscriptions.Application.Commands
{
    public sealed record SubscriptionRemove2Command(string SubscriberId, string NexusModsUrl) : IRequest<bool>;
}