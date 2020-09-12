using System;

namespace NexusMods.Monitor.Subscriptions.Domain.Exceptions
{
    public class MonitorSubscriptionsDomainException : Exception
    {
        public MonitorSubscriptionsDomainException() { }
        public MonitorSubscriptionsDomainException(string message) : base(message) { }
        public MonitorSubscriptionsDomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}