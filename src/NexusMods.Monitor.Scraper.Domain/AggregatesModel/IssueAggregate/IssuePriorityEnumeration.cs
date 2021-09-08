using NexusMods.Monitor.Shared.Domain.SeedWork;

using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    public sealed record IssuePriorityEnumeration(uint Id, string Name) : Enumeration(Id, Name)
    {
        public static IEnumerable<IssuePriorityEnumeration> List() => new IssuePriorityEnumeration[]
        {
            new(1, "ERROR"),
            new(2, "Not Set"),
            new(3, "Low"),
            new(4, "Medium"),
            new(5, "High")
        };
    }
}