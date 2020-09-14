using NodaTime;

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NexusMods.Monitor.Scraper.Application.Queries.Issues
{
    [DataContract]
    public sealed class IssueViewModel
    {
        [DataMember]
        public uint Id { get; private set; } = default!;
        [DataMember]
        public uint NexusModsGameId { get; private set; } = default!;
        [DataMember]
        public uint NexusModsModId { get; private set; } = default!;
        [DataMember]
        public int Status { get; private set; } = default!;
        [DataMember]
        public int Priority { get; private set; } = default!;
        [DataMember]
        public bool IsClosed { get; private set; } = default!;
        [DataMember]
        public bool IsPrivate { get; private set; } = default!;
        [DataMember]
        public Instant TimeOfLastPost { get; private set; } = default!;

        [DataMember]
        private readonly List<IssueReplyViewModel> _replies;
        public IEnumerable<IssueReplyViewModel> Replies => _replies;

        private IssueViewModel()
        {
            _replies = new List<IssueReplyViewModel>();
        }
        public IssueViewModel(uint id, uint nexusModsGameId, uint nexusModsModId, int status, int priority, bool isClosed, bool isPrivate, Instant timeOfLastPost, IEnumerable<IssueReplyViewModel> issueReplies) : this()
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