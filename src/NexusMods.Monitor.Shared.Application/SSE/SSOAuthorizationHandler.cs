using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Application.SSE
{
    public sealed class SSOAuthorizationHandler : ISSOAuthorizationHandler
    {
        public event Func<Task>? OnReadyAsync;
        public event Func<Task>? OnErrorAsync;
        public event Func<Task>? OnAuthorizedAsync;

        private readonly TaskCompletionSource _taskCompletionSource;
        private readonly EventSourceReader _eventSourceReader;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public SSOAuthorizationHandler(Guid id, IHttpClientFactory httpClientFactory)
        {
            _taskCompletionSource = new TaskCompletionSource();
            _eventSourceReader = new EventSourceReader(httpClientFactory.CreateClient("Metadata.API"), $"sso-authorize?uuid={id}");
            _eventSourceReader.MessageReceived += (_, args) =>
            {
                if (args.Event == "Connected")
                {
                    OnReadyAsync?.Invoke();
                }
                if (args.Event == "Closed")
                {
                    OnAuthorizedAsync?.Invoke();
                }
            };
            _eventSourceReader.Disconnected += (sender, e) =>
            {
                _taskCompletionSource?.SetResult();
            };
            _cancellationTokenSource = new CancellationTokenSource(60000);
            _cancellationTokenSource.Token.Register(() =>
            {
                _taskCompletionSource?.SetResult();
            });
        }

        public async Task<SSOAuthorizationHandler> StartAsync(CancellationToken ct)
        {
            await _eventSourceReader.StartAsync(ct);

            return this;
        }

        public TaskAwaiter GetAwaiter() => _taskCompletionSource.Task.GetAwaiter();

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
            _eventSourceReader.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            _cancellationTokenSource.Dispose();
            await _eventSourceReader.DisposeAsync();
        }
    }
}