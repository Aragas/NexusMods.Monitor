namespace NexusMods.Monitor.Scraper.Application.Options
{
    public sealed record NexusModsOptions
    {
        public string APIKey { get; set; } = default!;
    }
}