using NexusMods.Monitor.Shared.Domain.Exceptions;

using System;

namespace NexusMods.Monitor.Scraper.Domain.Exceptions
{
    public class MonitorScraperDomainException : DomainException
    {
        public MonitorScraperDomainException() { }
        public MonitorScraperDomainException(string message) : base(message) { }
        public MonitorScraperDomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}