using MediatR;

namespace NexusMods.Monitor.Subscriptions.Application.Commands
{
    public sealed record SubscriptionAdd2Command(string SubscriberId, string NexusModsUrl) : IRequest<bool>;
}