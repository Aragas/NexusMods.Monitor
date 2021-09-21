using MediatR;

using Microsoft.EntityFrameworkCore;

using NexusMods.Monitor.Shared.Domain.SeedWork;
using NexusMods.Monitor.Shared.Infrastructure.Extensions;
using NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Subscriptions.Infrastructure.Contexts.Config;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.Infrastructure.Contexts
{
    public sealed class SubscriptionDb : DbContext, IUnitOfWork
    {
        private readonly IMediator? _mediator;

        public DbSet<SubscriptionEntity> SubscriptionEntities { get; set; } = default!;

        public SubscriptionDb(DbContextOptions<SubscriptionDb> options) : base(options) { }
        public SubscriptionDb(DbContextOptions<SubscriptionDb> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new SubscriptionEntityConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken ct = default)
        {
            // Dispatch Domain Events collection.
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions.
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers.
            if (_mediator is not null)
                await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers)
            // performed through the DbContext will be committed
            _ = await base.SaveChangesAsync(ct);

            return true;
        }
    }
}