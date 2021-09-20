namespace NexusMods.Monitor.Shared.Application.SSE
{
    public record EventSourceMessageEventArgs(string Id, string Event, string Message);
}