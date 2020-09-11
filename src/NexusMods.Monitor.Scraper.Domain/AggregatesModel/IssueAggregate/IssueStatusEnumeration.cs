using NexusMods.Monitor.Scraper.Domain.Exceptions;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    public sealed class IssueStatusEnumeration : Enumeration
    {
        public static readonly IssueStatusEnumeration None = new IssueStatusEnumeration(1, "ERROR");
        public static readonly IssueStatusEnumeration NewIssue = new IssueStatusEnumeration(2, "New Issue");
        public static readonly IssueStatusEnumeration BeingLookedAt = new IssueStatusEnumeration(3, "Being Looked At");
        public static readonly IssueStatusEnumeration Fixed = new IssueStatusEnumeration(4, "Fixed");
        public static readonly IssueStatusEnumeration KnownIssue = new IssueStatusEnumeration(5, "Known Issue");
        public static readonly IssueStatusEnumeration Duplicate = new IssueStatusEnumeration(6, "Duplicate");
        public static readonly IssueStatusEnumeration NotABug = new IssueStatusEnumeration(7, "Not a Bug");
        public static readonly IssueStatusEnumeration WontFix = new IssueStatusEnumeration(8, "Won't Fix");
        public static readonly IssueStatusEnumeration NeedsMoreInfo = new IssueStatusEnumeration(9, "Needs More Info");

        private IssueStatusEnumeration() { }
        public IssueStatusEnumeration(int id, string name) : base(id, name) { }

        public static IEnumerable<IssueStatusEnumeration> List() => new[] { None, NewIssue, BeingLookedAt, Fixed, KnownIssue, Duplicate, NotABug, WontFix, NeedsMoreInfo };

        public static IssueStatusEnumeration FromName(string name)
        {
            var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (state is null)
            {
                throw new MonitorDomainException($"Possible values for IssueStatus: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }

        public static IssueStatusEnumeration From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);
            if (state is null)
            {
                throw new MonitorDomainException($"Possible values for IssueStatus: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }
    }
}