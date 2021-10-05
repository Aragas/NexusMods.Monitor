using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public interface IIssueIntegrationEventPublisher
    {
        Task Publish(IssueIntegrationEvent issueEvent, CancellationToken ct);
    }
}
