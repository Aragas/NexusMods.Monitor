namespace NexusMods.Monitor.Bot.Discord.Host.Options
{
    public sealed record SubscriptionsAPIOptions
    {
        public string APIEndpointV1 { get; set; } = default!;
    }
}