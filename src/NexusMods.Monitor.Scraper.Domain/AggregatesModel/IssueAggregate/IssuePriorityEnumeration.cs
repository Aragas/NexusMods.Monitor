using NexusMods.Monitor.Shared.Domain.SeedWork;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    public sealed record IssuePriorityEnumeration(uint Id, string Name) : Enumeration(Id, Name)
    {
        public static readonly IssuePriorityEnumeration Error  = new(1, "ERROR");
        public static readonly IssuePriorityEnumeration NotSet = new(2, "Not Set");
        public static readonly IssuePriorityEnumeration Low    = new(3, "Low");
        public static readonly IssuePriorityEnumeration Medium = new(4, "Medium");
        public static readonly IssuePriorityEnumeration High   = new(5, "High");
    }
}