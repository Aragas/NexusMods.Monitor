using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public sealed class NexusModsIssueRootViewModel
    {
        [DataMember]
        public string NexusModsGameIdText { get; private set; } = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsModId { get; private set; } = default!;
        [DataMember]
        public NexusModsIssueViewModel NexusModsIssue { get; private set; } = default!;
        [DataMember]
        public NexusModsIssueContentViewModel? NexusModsIssueContent { get; private set; } = default!;

        [DataMember]
        private readonly List<NexusModsIssueReplyViewModel> _nexusModsIssueReplies;
        public IEnumerable<NexusModsIssueReplyViewModel> NexusModsIssueReplies => _nexusModsIssueReplies;

        private NexusModsIssueRootViewModel()
        {
            _nexusModsIssueReplies = new List<NexusModsIssueReplyViewModel>();
        }
        public NexusModsIssueRootViewModel(string nexusModsGameIdText, uint nexusModsGameId, uint nexusModsModId, NexusModsIssueViewModel nexusModsIssue) : this()
        {
            NexusModsGameIdText = nexusModsGameIdText;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            NexusModsIssue = nexusModsIssue;
        }

        public void SetContent(NexusModsIssueContentViewModel? content) => NexusModsIssueContent = content;
        public async Task SetReplies(IAsyncEnumerable<NexusModsIssueReplyViewModel> replies) => _nexusModsIssueReplies.AddRange(await replies.ToListAsync());
        public void SetReplies(IEnumerable<NexusModsIssueReplyViewModel> replies) => _nexusModsIssueReplies.AddRange(replies);
    }
}