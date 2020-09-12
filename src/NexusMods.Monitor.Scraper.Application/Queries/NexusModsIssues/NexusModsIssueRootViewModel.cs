using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public class NexusModsIssueRootViewModel
    {
        public string NexusModsGameIdText { get; }
        public uint NexusModsGameId { get; }
        public uint NexusModsModId { get; }
        public NexusModsIssueViewModel NexusModsIssue { get; }
        public NexusModsIssueContentViewModel? NexusModsIssueContent { get; private set; }
        public List<NexusModsIssueReplyViewModel>? NexusModsIssueReplies { get; private set; }

        public NexusModsIssueRootViewModel(string nexusModsGameIdText, uint nexusModsGameId, uint nexusModsModId, NexusModsIssueViewModel nexusModsIssue)
        {
            NexusModsGameIdText = nexusModsGameIdText;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            NexusModsIssue = nexusModsIssue;
        }

        public void SetContent(NexusModsIssueContentViewModel? content) => NexusModsIssueContent = content;
        public async Task SetReplies(IAsyncEnumerable<NexusModsIssueReplyViewModel> replies) => NexusModsIssueReplies = await replies.ToListAsync();
        public void SetReplies(IEnumerable<NexusModsIssueReplyViewModel> replies) => NexusModsIssueReplies = replies.ToList();
    }
}