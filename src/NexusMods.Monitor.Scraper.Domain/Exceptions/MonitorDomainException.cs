using System;

namespace NexusMods.Monitor.Scraper.Domain.Exceptions
{
    public class MonitorDomainException : Exception
    {
        public MonitorDomainException() { }
        public MonitorDomainException(string message) : base(message) { }
        public MonitorDomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}