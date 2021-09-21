using FluentValidation;

using NexusMods.Monitor.Shared.Application.Extensions;

namespace NexusMods.Monitor.Bot.Discord.Host.Options
{
    public sealed class DiscordOptionsValidator : AbstractValidator<DiscordOptions>
    {
        public DiscordOptionsValidator()
        {
            RuleFor(options => options.BotToken).NotEmpty().NotInteger().NotBoolean();
        }
    }

    public sealed record DiscordOptions
    {
        public string BotToken { get; init; } = default!;
    }
}