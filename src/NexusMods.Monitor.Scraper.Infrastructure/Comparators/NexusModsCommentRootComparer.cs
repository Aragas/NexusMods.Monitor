using NexusMods.Monitor.Scraper.Infrastructure.Models.Comments;

using System;
using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Infrastructure.Comparators
{
    public class NexusModsCommentRootComparer : IEqualityComparer<NexusModsCommentRoot>
    {
        public bool Equals(NexusModsCommentRoot? x, NexusModsCommentRoot? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.NexusModsComment.Id == y.NexusModsComment.Id;
        }

        public int GetHashCode(NexusModsCommentRoot obj)
        {
            return HashCode.Combine(obj.NexusModsComment.Id);
        }
    }
}