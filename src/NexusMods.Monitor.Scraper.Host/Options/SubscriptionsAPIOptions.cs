namespace NexusMods.Monitor.Scraper.Host.Options
{
    public sealed record SubscriptionsAPIOptions
    {
        public string APIEndpointV1 { get; set; } = default!;
    }
}