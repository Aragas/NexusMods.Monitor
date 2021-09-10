using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.Queries.Authorizations
{
    public interface IAuthorizationQueries
    {
        Task<bool> IsAuthorizedAsync(CancellationToken ct = default);
    }
}