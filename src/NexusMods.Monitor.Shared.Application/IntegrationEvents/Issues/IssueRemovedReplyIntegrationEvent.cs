using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed class IssueRemovedReplyIntegrationEvent : Event
    {
        public IssueDTO Issue { get; private set; } = default!;
        public IssueDTO.IssueReplyDTO IssueReply { get; private set; } = default!;

        private IssueRemovedReplyIntegrationEvent() { }
        public IssueRemovedReplyIntegrationEvent(IssueDTO issue, IssueDTO.IssueReplyDTO issueReply) : this()
        {
            Issue = issue;
            IssueReply = issueReply;
        }
    }
}