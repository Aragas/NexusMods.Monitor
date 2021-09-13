using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.Threads
{
    public interface IThreadQueries
    {
        Task<ThreadViewModel?> GetAsync(uint gameId, uint modId, CancellationToken ct = default);
    }
}