using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Domain.SeedWork
{
    public sealed class ReadOnlyUnitOfWork : IUnitOfWork
    {
        public static ReadOnlyUnitOfWork Instance { get; } = new ReadOnlyUnitOfWork();

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);

        public void Dispose() { }
    }
}