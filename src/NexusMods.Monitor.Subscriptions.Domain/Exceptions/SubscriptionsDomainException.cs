using NexusMods.Monitor.Shared.Domain.Exceptions;

using System;

namespace NexusMods.Monitor.Subscriptions.Domain.Exceptions
{
    public class SubscriptionsDomainException : DomainException
    {
        public SubscriptionsDomainException() { }
        public SubscriptionsDomainException(string message) : base(message) { }
        public SubscriptionsDomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}