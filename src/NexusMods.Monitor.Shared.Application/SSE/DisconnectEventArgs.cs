using System;

namespace NexusMods.Monitor.Shared.Application.SSE
{
    public class DisconnectEventArgs : EventArgs
    {
        /// <summary>
        /// Reconnect delay requested by server
        /// </summary>
        public int ReconnectDelay { get; }

        /// <summary>
        /// Exception that caused the disconnect, null if graceful disconnect.
        /// </summary>
        public Exception Exception { get; }

        internal DisconnectEventArgs(int reconnectDelay, Exception exception)
        {
            ReconnectDelay = reconnectDelay;
            Exception = exception;
        }
    }
}