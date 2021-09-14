namespace NexusMods.Monitor.Bot.Slack.Host.Options
{
    public sealed record MetadataAPIOptions
    {
        public string APIEndpointV1 { get; init; } = default!;
    }
}