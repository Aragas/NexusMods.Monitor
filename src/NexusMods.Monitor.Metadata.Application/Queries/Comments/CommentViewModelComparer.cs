using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Metadata.Application.Queries.Comments
{
    internal sealed class CommentViewModelComparer : IEqualityComparer<CommentViewModel>
    {
        public bool Equals(CommentViewModel? x, CommentViewModel? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(CommentViewModel obj)
        {
            return HashCode.Combine(obj.Id);
        }
    }
}