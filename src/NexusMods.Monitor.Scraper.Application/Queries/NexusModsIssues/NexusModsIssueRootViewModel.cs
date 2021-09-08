using System.Collections.Generic;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public sealed record NexusModsIssueRootViewModel(string GameDomain, uint GameId, uint ModId, string GameName, string ModName, NexusModsIssueViewModel NexusModsIssue)
    {
        public NexusModsIssueContentViewModel? NexusModsIssueContent { get; private set; }

        private readonly List<NexusModsIssueReplyViewModel> _nexusModsIssueReplies = new();
        public IReadOnlyList<NexusModsIssueReplyViewModel> NexusModsIssueReplies => _nexusModsIssueReplies.AsReadOnly();

        public void SetContent(NexusModsIssueContentViewModel? content) => NexusModsIssueContent = content;

        public async Task SetRepliesAsync(IAsyncEnumerable<NexusModsIssueReplyViewModel> replies)
        {
            await foreach (var reply in replies)
            {
                _nexusModsIssueReplies.Add(reply);
            }
        }
        public void SetReplies(IEnumerable<NexusModsIssueReplyViewModel> replies) => _nexusModsIssueReplies.AddRange(replies);
    }
}