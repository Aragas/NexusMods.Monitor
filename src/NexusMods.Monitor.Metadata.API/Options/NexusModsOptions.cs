namespace NexusMods.Monitor.Metadata.API.Options
{
    public sealed record NexusModsOptions
    {
        public string Endpoint { get; set; } = default!;
        public string APIKey { get; set; } = default!;
    }
}