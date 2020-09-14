using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public sealed class NexusModsIssueRootViewModelComparer : IEqualityComparer<NexusModsIssueRootViewModel>
    {
        public bool Equals(NexusModsIssueRootViewModel? x, NexusModsIssueRootViewModel? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.NexusModsIssue.Id == y.NexusModsIssue.Id;
        }

        public int GetHashCode(NexusModsIssueRootViewModel obj)
        {
            return HashCode.Combine(obj.NexusModsIssue.Id);
        }
    }
}