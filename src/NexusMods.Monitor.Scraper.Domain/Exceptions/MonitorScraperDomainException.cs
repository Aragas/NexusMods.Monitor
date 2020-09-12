using System;

namespace NexusMods.Monitor.Scraper.Domain.Exceptions
{
    public class MonitorScraperDomainException : Exception
    {
        public MonitorScraperDomainException() { }
        public MonitorScraperDomainException(string message) : base(message) { }
        public MonitorScraperDomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}