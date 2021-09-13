using System;

namespace NexusMods.Monitor.Shared.Application.SSE
{
    public class EventSourceMessageEventArgs : EventArgs
    {
        /// <summary>
        /// ID of the event, empty string if not present
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Event type, empty string if not present
        /// </summary>
        public string Event { get; }

        /// <summary>
        /// Event data
        /// </summary>
        public string Message { get; }

        internal EventSourceMessageEventArgs(string data, string type, string id)
        {
            Message = data;
            Event = type;
            Id = id;
        }
    }
}