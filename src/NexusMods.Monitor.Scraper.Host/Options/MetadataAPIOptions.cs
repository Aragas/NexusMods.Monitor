namespace NexusMods.Monitor.Scraper.Host.Options
{
    public sealed record MetadataAPIOptions
    {
        public string APIEndpointV1 { get; init; } = default!;
    }
}