using FluentValidation;

namespace NexusMods.Monitor.Bot.Slack.Host.Options
{
    public sealed class SlackOptionsValidator : AbstractValidator<SlackOptions>
    {
        public SlackOptionsValidator()
        {
            RuleFor(options => options.BotToken).NotEmpty();
        }
    }

    public sealed record SlackOptions
    {
        public string BotToken { get; init; } = default!;
    }
}