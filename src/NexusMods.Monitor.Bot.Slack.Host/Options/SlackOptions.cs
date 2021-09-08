namespace NexusMods.Monitor.Bot.Slack.Host.Options
{
    public sealed record SlackOptions
    {
        public string BotToken { get; set; } = default!;
    }
}