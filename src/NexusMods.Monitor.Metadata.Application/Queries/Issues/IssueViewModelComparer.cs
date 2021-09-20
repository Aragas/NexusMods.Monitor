using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Metadata.Application.Queries.Issues
{
    internal sealed class IssueViewModelComparer : IEqualityComparer<IssueViewModel>
    {
        public bool Equals(IssueViewModel? x, IssueViewModel? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(IssueViewModel obj) => HashCode.Combine(obj.Id);
    }
}