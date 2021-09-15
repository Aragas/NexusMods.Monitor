using Enbiso.NLib.EventBus.Nats;

using FluentValidation;

namespace NexusMods.Monitor.Shared.Host.Options
{
    public sealed class NatsOptionsValidator : AbstractValidator<NatsOptions>
    {
        public NatsOptionsValidator()
        {
            RuleFor(options => options.Servers).NotEmpty();
        }
    }
}