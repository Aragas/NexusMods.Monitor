namespace NexusMods.Monitor.Scraper.Application.Options
{
    public sealed record SubscriptionsOptions
    {
        public string? APIEndpointV1 { get; set; } = default!;
    }
}