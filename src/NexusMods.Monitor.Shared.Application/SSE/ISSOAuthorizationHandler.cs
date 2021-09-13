using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Application.SSE
{
    public interface ISSOAuthorizationHandler : IDisposable, IAsyncDisposable
    {
        event Func<Task>? OnReadyAsync;
        event Func<Task>? OnErrorAsync;
        event Func<Task>? OnAuthorizedAsync;
        TaskAwaiter GetAwaiter();
    }
}