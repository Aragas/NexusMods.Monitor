using System;

namespace NexusMods.Monitor.Shared.Application.SSE
{
    public record DisconnectEventArgs(int ReconnectDelay, Exception Exception);
}