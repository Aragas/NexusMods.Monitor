using NexusMods.Monitor.Scraper.Infrastructure.Models.Issues;

using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Infrastructure.Comparators
{
    public class NexusModsIssueRootComparer : IEqualityComparer<NexusModsIssueRoot>
    {
        public bool Equals(NexusModsIssueRoot? x, NexusModsIssueRoot? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.NexusModsIssue.Id == y.NexusModsIssue.Id;
        }

        public int GetHashCode(NexusModsIssueRoot obj)
        {
            return HashCode.Combine(obj.NexusModsIssue.Id);
        }
    }
}