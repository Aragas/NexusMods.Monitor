using System;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Common.Extensions
{
    public static class CommonExtensions
    {
        public static void TryDispose(this object obj)
        {
            if (obj is IDisposable disposable)
                disposable.Dispose();
        }

        public static ValueTask TryDisposeAsync(this object obj)
        {
            if (obj is IAsyncDisposable asyncDisposable)
                return asyncDisposable.DisposeAsync();

            if (obj is IDisposable disposable)
                disposable.Dispose();

            return ValueTask.CompletedTask;
        }
    }
}