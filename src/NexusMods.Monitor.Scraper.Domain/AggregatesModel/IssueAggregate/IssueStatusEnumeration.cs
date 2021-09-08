using NexusMods.Monitor.Shared.Domain.SeedWork;

using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    public sealed record IssueStatusEnumeration(uint Id, string Name) : Enumeration(Id, Name)
    {
        public static IEnumerable<IssueStatusEnumeration> List() => new IssueStatusEnumeration[]
        {
            new(1, "ERROR"),
            new(2, "New Issue"),
            new(3, "Being Looked At"),
            new(4, "Fixed"),
            new(5, "Known Issue"),
            new(6, "Duplicate"),
            new(7, "Not a Bug"),
            new(8, "Won't Fix"),
            new(9, "Needs More Info")
        };
    }
}