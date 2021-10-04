using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Application.SSE
{
    public sealed class EventSourceReader : IDisposable, IAsyncDisposable
    {
        private const string DefaultEventType = "message";

        public delegate void MessageReceivedHandler(object sender, EventSourceMessageEventArgs e);
        public delegate void DisconnectEventHandler(object sender, DisconnectEventArgs e);

        public event MessageReceivedHandler? MessageReceived;
        public event DisconnectEventHandler? Disconnected;

        private readonly HttpClient _httpClient;
        private readonly string _uri;
        private readonly object _startLock = new();

        private bool _isReading;
        private int _reconnectDelay = 3000;
        private string _lastEventId = string.Empty;

        public EventSourceReader(HttpClient httpClient, string url)
        {
            _httpClient = httpClient;
            _uri = url;
        }

        public ValueTask<EventSourceReader> StartAsync(CancellationToken ct)
        {
            lock (_startLock)
            {
                if (_isReading == false)
                {
                    _isReading = true;
                    // Only start a new one if one isn't already running
                    _ = ReaderAsync(ct);
                }
            }
            return ValueTask.FromResult(this);
        }

        private async Task ReaderAsync(CancellationToken ct)
        {
            try
            {
                if (string.Empty != _lastEventId)
                {
                    if (_httpClient.DefaultRequestHeaders.Contains("Last-Event-Id"))
                    {
                        _httpClient.DefaultRequestHeaders.Remove("Last-Event-Id");
                    }

                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Last-Event-Id", _lastEventId);
                }

                using var response = await _httpClient.GetAsync(_uri, HttpCompletionOption.ResponseHeadersRead, ct);
                response.EnsureSuccessStatusCode();
                if (response.Headers.TryGetValues("content-type", out var ctypes) || ctypes?.Contains("text/event-stream") == false)
                {
                    throw new ArgumentException("Specified URI does not return server-sent events");
                }

                await using var stream = await response.Content.ReadAsStreamAsync(ct);
                using var sr = new StreamReader(stream);

                var evt = DefaultEventType;
                var id = string.Empty;
                var data = new StringBuilder();

                while (!ct.IsCancellationRequested)
                {
                    var line = await sr.ReadLineAsync();

                    if (string.IsNullOrEmpty(line))
                    {
                        // double newline, dispatch message and reset for next
                        if (data.Length > 0)
                        {
                            MessageReceived?.Invoke(this, new EventSourceMessageEventArgs(data.ToString().Trim(), evt, id));
                        }
                        data.Clear();
                        id = string.Empty;
                        evt = DefaultEventType;
                        continue;
                    }

                    if (line.First() == ':')
                    {
                        // Ignore comments
                        continue;
                    }

                    var dataIndex = line.IndexOf(':');
                    string field;
                    if (dataIndex == -1)
                    {
                        dataIndex = line.Length;
                        field = line;
                    }
                    else
                    {
                        field = line.Substring(0, dataIndex);
                        dataIndex += 1;
                    }

                    var value = line.Substring(dataIndex).Trim();

                    switch (field)
                    {
                        case "event":
                            // Set event type
                            evt = value;
                            break;
                        case "data":
                            // Append a line to data using a single \n as EOL
                            data.Append($"{value}\n");
                            break;
                        case "retry":
                            // Set reconnect delay for next disconnect
                            _reconnectDelay = int.TryParse(value, out var reconnectDelay) ? reconnectDelay : -1;
                            break;
                        case "id":
                            // Set ID
                            _lastEventId = value;
                            id = _lastEventId;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _isReading = false;
                Disconnected?.Invoke(this, new DisconnectEventArgs(_reconnectDelay, ex));
            }
        }

        /// <summary>
        /// Stop and dispose of the EventSourceReader
        /// </summary>
        public void Dispose()
        {
            _httpClient.CancelPendingRequests();
            _httpClient.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            _httpClient.CancelPendingRequests();
            _httpClient.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}