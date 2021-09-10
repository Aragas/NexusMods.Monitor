namespace NexusMods.Monitor.Bot.Discord.Host.Options
{
    public sealed record MetadataAPIOptions
    {
        public string APIEndpointV1 { get; init; } = default!;
    }
}