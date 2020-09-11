using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsThreadAggregate;

namespace NexusMods.Monitor.Scraper.Infrastructure.Contexts.Config
{
    public sealed class NexusModsThreadEntityConfiguration : IEntityTypeConfiguration<NexusModsThreadEntity>
    {
        public void Configure(EntityTypeBuilder<NexusModsThreadEntity> builder)
        {
            builder.ToTable("thread_id_entity").HasKey(p => new { p.NexusModsGameId, p.NexusModsModId });
            builder.Property(p => p.NexusModsGameId).HasColumnName("game_id").ValueGeneratedNever().IsRequired();
            builder.Property(p => p.NexusModsModId).HasColumnName("mod_id").ValueGeneratedNever().IsRequired();
            builder.Property(p => p.ThreadId).HasColumnName("thread_id").IsRequired();
        }
    }
}