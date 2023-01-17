using NexusMods.Monitor.Shared.Domain.SeedWork;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    public sealed record IssueStatusEnumeration(uint Id, string Name) : Enumeration(Id, Name)
    {
        public static readonly IssueStatusEnumeration Error         = new(1, "ERROR");
        public static readonly IssueStatusEnumeration NewIssue      = new(2, "New Issue");
        public static readonly IssueStatusEnumeration BeingLookedAt = new(3, "Being Looked At");
        public static readonly IssueStatusEnumeration Fixed         = new(4, "Fixed");
        public static readonly IssueStatusEnumeration KnownIssue    = new(5, "Known Issue");
        public static readonly IssueStatusEnumeration Duplicate     = new(6, "Duplicate");
        public static readonly IssueStatusEnumeration NotABug       = new(7, "Not a Bug");
        public static readonly IssueStatusEnumeration WontFix       = new(8, "Won't Fix");
        public static readonly IssueStatusEnumeration NeedsMoreInfo = new(9, "Needs More Info");
    }
}