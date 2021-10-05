using NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues;

using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Comments
{
    public interface ICommentIntegrationEventPublisher
    {
        Task Publish(CommentIntegrationEvent commentEvent, CancellationToken ct);
    }
}
