using MediatR;

using Microsoft.EntityFrameworkCore;

using NexusMods.Monitor.Shared.Domain.SeedWork;

using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Infrastructure.Extensions
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<DefaultEntity>()
                .Where(x => x.Entity.DomainEvents.Count > 0)
                .ToImmutableArray();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToImmutableArray();

            foreach (var domainEntity in domainEntities)
                domainEntity.Entity.ClearDomainEvents();

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}