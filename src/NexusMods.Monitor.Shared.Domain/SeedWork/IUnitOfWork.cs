using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Domain.SeedWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> SaveEntitiesAsync(CancellationToken ct = default);
    }
}