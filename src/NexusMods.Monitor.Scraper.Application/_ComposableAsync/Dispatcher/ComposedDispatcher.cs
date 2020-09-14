using System;
using System.Threading;
using System.Threading.Tasks;

namespace ComposableAsync
{
    internal class ComposedDispatcher : IDispatcher, IAsyncDisposable
    {
        private readonly IDispatcher _First;
        private readonly IDispatcher _Second;

        public ComposedDispatcher(IDispatcher first, IDispatcher second)
        {
            _First = first;
            _Second = second;
        }

        public void Dispatch(Action action)
        {
            _First.Dispatch(() => _Second.Dispatch(action));
        }

        public async Task Enqueue(Action action)
        {
            await _First.Enqueue(() => _Second.Enqueue(action));
        }

        public async Task<T> Enqueue<T>(Func<T> action)
        {
            return await _First.Enqueue(() => _Second.Enqueue(action));
        }

        public async Task Enqueue(Func<Task> action)
        {
            await _First.Enqueue(() => _Second.Enqueue(action));
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> action)
        {
            return await _First.Enqueue(() => _Second.Enqueue(action));
        }

        public async Task Enqueue(Func<Task> action, CancellationToken cancellationToken)
        {
            await _First.Enqueue(() => _Second.Enqueue(action, cancellationToken), cancellationToken);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> action, CancellationToken cancellationToken)
        {
            return await _First.Enqueue(() => _Second.Enqueue(action, cancellationToken), cancellationToken);
        }

        public async Task<T> Enqueue<T>(Func<T> action, CancellationToken cancellationToken)
        {
            return await _First.Enqueue(() => _Second.Enqueue(action, cancellationToken), cancellationToken);
        }

        public async Task Enqueue(Action action, CancellationToken cancellationToken)
        {
            await _First.Enqueue(() => _Second.Enqueue(action, cancellationToken), cancellationToken);
        }

        public IDispatcher Clone() => new ComposedDispatcher(_First, _Second);

        public ValueTask DisposeAsync() => new ValueTask(Task.WhenAll(DisposeAsync(_First).AsTask(), DisposeAsync(_Second).AsTask()));

        private static ValueTask DisposeAsync(IDispatcher disposable) => (disposable as IAsyncDisposable)?.DisposeAsync() ?? new ValueTask(Task.CompletedTask);

    }
}