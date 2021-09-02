namespace NexusMods.Monitor.Bot.Slack.Application.Options
{
    public sealed record SubscriptionsOptions
    {
        public string APIEndpointV1 { get; set; } = default!;
    }
}