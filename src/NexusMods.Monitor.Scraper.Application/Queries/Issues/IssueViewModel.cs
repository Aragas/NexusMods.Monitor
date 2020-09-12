using NodaTime;

using System.Collections.Generic;

namespace NexusMods.Monitor.Scraper.Application.Queries.Issues
{
    public sealed class IssueViewModel
    {
        public uint Id { get; }
        public uint NexusModsGameId { get; }
        public uint NexusModsModId { get; }
        public int Status { get; }
        public int Priority { get; }
        public bool IsClosed { get; }
        public bool IsPrivate { get; }
        public Instant TimeOfLastPost { get; }

        private readonly List<IssueReplyViewModel> _replies;
        public IEnumerable<IssueReplyViewModel> Replies => _replies;

        private IssueViewModel()
        {
            _replies = new List<IssueReplyViewModel>();
        }
        public IssueViewModel(uint id, uint nexusModsGameId, uint nexusModsModId, int status, int priority, bool isClosed, bool isPrivate, Instant timeOfLastPost, IEnumerable<IssueReplyViewModel> issueReplies)
        {
            Id = id;
            NexusModsGameId = nexusModsGameId;
            NexusModsModId = nexusModsModId;
            Status = status;
            Priority = priority;
            IsClosed = isClosed;
            IsPrivate = isPrivate;
            TimeOfLastPost = timeOfLastPost;
            _replies.AddRange(issueReplies);
        }
    }
}