using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate;

namespace NexusMods.Monitor.Subscriptions.Infrastructure.Contexts.Config
{
    public sealed class SubscriptionEntityConfiguration : IEntityTypeConfiguration<SubscriptionEntity>
    {
        public void Configure(EntityTypeBuilder<SubscriptionEntity> builder)
        {
            builder.ToTable("subscription_entity").HasKey(p => new { p.SubscriberId, p.NexusModsGameId, p.NexusModsModId });
            builder.Property(p => p.SubscriberId).HasColumnName("subscriber_id").IsRequired();
            builder.Property(p => p.NexusModsGameId).HasColumnName("game_id").IsRequired();
            builder.Property(p => p.NexusModsModId).HasColumnName("mod_id").IsRequired();
        }
    }
}