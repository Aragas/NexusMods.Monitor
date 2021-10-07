using System.Diagnostics.CodeAnalysis;

namespace NexusMods.Monitor.Shared.Domain.SeedWork
{
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}