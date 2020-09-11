using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Infrastructure.Models.Comments
{
    public interface INexusModsCommentsRepository
    {
        IAsyncEnumerable<NexusModsCommentRoot> GetCommentsAsync(uint gameId, uint modId);
    }
}