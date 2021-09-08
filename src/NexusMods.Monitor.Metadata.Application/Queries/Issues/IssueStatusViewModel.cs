namespace NexusMods.Monitor.Metadata.Application.Queries.Issues
{
    public sealed record IssueStatusViewModel(uint Id, string Name)
    {
        public static readonly IssueStatusViewModel None = new(1, "ERROR");
        public static readonly IssueStatusViewModel NewIssue = new(2, "New Issue");
        public static readonly IssueStatusViewModel BeingLookedAt = new(3, "Being Looked At");
        public static readonly IssueStatusViewModel Fixed = new(4, "Fixed");
        public static readonly IssueStatusViewModel KnownIssue = new(5, "Known Issue");
        public static readonly IssueStatusViewModel Duplicate = new(6, "Duplicate");
        public static readonly IssueStatusViewModel NotABug = new(7, "Not a Bug");
        public static readonly IssueStatusViewModel WontFix = new(8, "Won't Fix");
        public static readonly IssueStatusViewModel NeedsMoreInfo = new(9, "Needs More Info");
    }
}