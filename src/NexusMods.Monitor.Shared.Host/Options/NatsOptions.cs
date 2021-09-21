using Enbiso.NLib.EventBus.Nats;

using FluentValidation;

using NexusMods.Monitor.Shared.Application.Extensions;

namespace NexusMods.Monitor.Shared.Host.Options
{
    public sealed class NatsOptionsValidator : AbstractValidator<NatsOptions>
    {
        public NatsOptionsValidator()
        {
            RuleFor(options => options.Servers).NotEmpty().IsNatsUri();
        }
    }
}