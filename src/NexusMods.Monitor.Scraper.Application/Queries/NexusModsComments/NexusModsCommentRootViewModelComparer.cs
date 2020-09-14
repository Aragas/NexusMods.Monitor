using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public sealed class NexusModsCommentRootViewModelComparer : IEqualityComparer<NexusModsCommentRootViewModel>
    {
        public bool Equals(NexusModsCommentRootViewModel? x, NexusModsCommentRootViewModel? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.NexusModsComment.Id == y.NexusModsComment.Id;
        }

        public int GetHashCode(NexusModsCommentRootViewModel obj)
        {
            return HashCode.Combine(obj.NexusModsComment.Id);
        }
    }
}