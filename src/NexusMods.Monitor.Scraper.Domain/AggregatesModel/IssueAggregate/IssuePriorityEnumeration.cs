using NexusMods.Monitor.Scraper.Domain.Exceptions;
using NexusMods.Monitor.Shared.Domain.SeedWork;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate
{
    public sealed class IssuePriorityEnumeration : Enumeration
    {
        public static readonly IssuePriorityEnumeration None = new(1, "ERROR");
        public static readonly IssuePriorityEnumeration NotSet = new(2, "Not Set");
        public static readonly IssuePriorityEnumeration Low = new(3, "Low");
        public static readonly IssuePriorityEnumeration Medium = new(4, "Medium");
        public static readonly IssuePriorityEnumeration High = new(5, "High");

        private IssuePriorityEnumeration() { }
        public IssuePriorityEnumeration(int id, string name) : base(id, name) { }

        public static IEnumerable<IssuePriorityEnumeration> List() => new[] { None, NotSet, Low, Medium, High };

        public static IssuePriorityEnumeration FromName(string name)
        {
            var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (state is null)
            {
                throw new MonitorScraperDomainException($"Possible values for IssuePriority: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }

        public static IssuePriorityEnumeration From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);
            if (state is null)
            {
                throw new MonitorScraperDomainException($"Possible values for IssuePriority: {string.Join(",", List().Select(s => s.Name))}");
            }
            return state;
        }
    }
}