namespace NexusMods.Monitor.Bot.Slack.Application.Options
{
    public sealed record SlackOptions
    {
        public string BotToken { get; set; } = default!;
    }
}