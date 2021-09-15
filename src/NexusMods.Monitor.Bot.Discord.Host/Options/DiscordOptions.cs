using FluentValidation;

namespace NexusMods.Monitor.Bot.Discord.Host.Options
{
    public sealed class DiscordOptionsValidator : AbstractValidator<DiscordOptions>
    {
        public DiscordOptionsValidator()
        {
            RuleFor(options => options.BotToken).NotEmpty();
        }
    }

    public sealed record DiscordOptions
    {
        public string BotToken { get; set; } = default!;
    }
}