using MediatR;

using Microsoft.EntityFrameworkCore;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Scraper.Infrastructure.Contexts.Config;
using NexusMods.Monitor.Shared.Domain.SeedWork;
using NexusMods.Monitor.Shared.Infrastructure.Extensions;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Infrastructure.Contexts
{
    public class NexusModsDb : DbContext, IUnitOfWork
    {
        private readonly IMediator? _mediator;

        public DbSet<IssueEntity> IssueEntities { get; set; } = default!;
        public DbSet<IssuePriorityEnumeration> IssuePriorityEnumerations { get; set; } = default!;
        public DbSet<IssueStatusEnumeration> IssueStatusEnumerations { get; set; } = default!;
        public DbSet<IssueContentEntity> IssueContentEntities { get; set; } = default!;
        public DbSet<IssueReplyEntity> IssueReplyEntities { get; set; } = default!;
        public DbSet<CommentEntity> CommentEntities { get; set; } = default!;
        public DbSet<CommentReplyEntity> CommentReplyEntities { get; set; } = default!;

        public NexusModsDb(DbContextOptions<NexusModsDb> options) : base(options) { }
        public NexusModsDb(DbContextOptions<NexusModsDb> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        protected NexusModsDb(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new IssueEntityConfiguration());
            modelBuilder.ApplyConfiguration(new IssuePriorityEnumerationConfiguration());
            modelBuilder.ApplyConfiguration(new IssueStatusEnumerationConfiguration());
            modelBuilder.ApplyConfiguration(new IssueContentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new IssueReplyEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CommentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CommentReplyEntityConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            if (!(_mediator is null))
                await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}