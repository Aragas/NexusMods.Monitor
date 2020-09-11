using Enbiso.NLib.EventBus;

namespace NexusMods.Monitor.Shared.Application.IntegrationEvents.Issues
{
    public sealed class IssueAddedReplyIntegrationEvent : Event
    {
        public IssueDTO Issue { get; private set; } = default!;
        public uint ReplyId { get; private set; } = default!;

        private IssueAddedReplyIntegrationEvent() { }
        public IssueAddedReplyIntegrationEvent(IssueDTO issue, uint replyId) : this()
        {
            Issue = issue;
            ReplyId = replyId;
        }
    }
}