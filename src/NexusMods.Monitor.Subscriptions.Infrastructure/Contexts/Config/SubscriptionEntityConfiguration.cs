using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate;

namespace NexusMods.Monitor.Subscriptions.Infrastructure.Contexts.Config
{
    public sealed class SubscriptionEntityConfiguration : IEntityTypeConfiguration<SubscriptionEntity>
    {
        public void Configure(EntityTypeBuilder<SubscriptionEntity> builder)
        {
            builder.Ignore(x => x.Id);
            builder.Property(typeof(string), nameof(SubscriptionEntity.SubscriberId)).HasColumnName("subscriber_id");
            builder.Property(typeof(uint), nameof(SubscriptionEntity.NexusModsGameId)).HasColumnName("game_id");
            builder.Property(typeof(uint), nameof(SubscriptionEntity.NexusModsModId)).HasColumnName("mod_id");
            builder.ToTable("subscription_entity", "subscriptions")
                .HasKey(nameof(SubscriptionEntity.SubscriberId), nameof(SubscriptionEntity.NexusModsGameId), nameof(SubscriptionEntity.NexusModsModId));
        }
    }
}