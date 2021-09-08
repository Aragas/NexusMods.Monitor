namespace NexusMods.Monitor.Metadata.Application.Queries.Issues
{
    public sealed record IssuePriorityViewModel(uint Id, string Name)
    {
        public static readonly IssuePriorityViewModel None = new(1, "ERROR");
        public static readonly IssuePriorityViewModel NotSet = new(2, "Not Set");
        public static readonly IssuePriorityViewModel Low = new(3, "Low");
        public static readonly IssuePriorityViewModel Medium = new(4, "Medium");
        public static readonly IssuePriorityViewModel High = new(5, "High");
    }
}