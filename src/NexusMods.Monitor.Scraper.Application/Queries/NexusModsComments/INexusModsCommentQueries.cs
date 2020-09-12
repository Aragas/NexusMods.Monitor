using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public interface INexusModsCommentQueries
    {
        IAsyncEnumerable<NexusModsCommentRootViewModel> GetAllAsync(uint gameId, uint modId);
    }
}