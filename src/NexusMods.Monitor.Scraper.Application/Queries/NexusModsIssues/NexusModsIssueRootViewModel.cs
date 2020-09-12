using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public class NexusModsIssueRootViewModel
    {
        public string NexusModsGameIdText { get; private set; } = default!;
        public uint NexusModsGameId { get; private set; } = default!;
        public uint NexusModsModId { get; private set; } = default!;
        public NexusModsIssueViewModel NexusModsIssue { get; private set; } = default!;
        public NexusModsIssueContentViewModel? NexusModsIssueContent { get; private set; } = default!;
        public List<NexusModsIssueReplyViewModel>? NexusModsIssueReplies { get; private set; } = default!;

        private NexusModsIssueRootViewModel() { }
        public NexusModsIssueRootViewModel(string nexusModsGameIdText, uint nexusModsGameId, uint nexusModsModId, NexusModsIssueViewModel nexusModsIssue) : this()
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