namespace NexusMods.Monitor.Bot.Discord.Host.Options
{
    public sealed record DiscordOptions
    {
        public string BotToken { get; set; } = default!;
    }
}