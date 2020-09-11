using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Infrastructure.Models.Issues
{
    public class NexusModsIssueRoot
    {
        public string NexusModsGameIdText { get; }
        public uint NexusModsGameId { get; }
        public uint NexusModsModId { get; }
        public NexusModsIssue NexusModsIssue { get; }
        public NexusModsIssueContent? NexusModsIssueContent { get; private set; }
        public List<NexusModsIssueReply>? NexusModsIssueReplies { get; private set; }

        public NexusModsIssueRoot(string nexusModsGameIdText, uint nexusModsGameId, uint nexusModsModId, NexusModsIssue nexusModsIssue)
        {
            NexusModsGameIdText = nexusModsGameIdText;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            NexusModsIssue = nexusModsIssue;
        }

        public void SetContent(NexusModsIssueContent? content) => NexusModsIssueContent = content;
        public async Task SetReplies(IAsyncEnumerable<NexusModsIssueReply> replies) => NexusModsIssueReplies = await replies.ToListAsync();
        public void SetReplies(IEnumerable<NexusModsIssueReply> replies) => NexusModsIssueReplies = replies.ToList();
    }
}