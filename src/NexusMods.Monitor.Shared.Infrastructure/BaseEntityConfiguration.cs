using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NexusMods.Monitor.Shared.Domain.SeedWork;

namespace NexusMods.Monitor.Shared.Infrastructure
{
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : DefaultEntity
    {
        public void Configure(EntityTypeBuilder<TEntity> entity)
        {
            ConfigureModel(entity);
        }

        protected abstract void ConfigureModel(EntityTypeBuilder<TEntity> entity);
    }
}