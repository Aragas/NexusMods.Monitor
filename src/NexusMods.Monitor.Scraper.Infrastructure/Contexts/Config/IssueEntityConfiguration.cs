using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Infrastructure;

namespace NexusMods.Monitor.Scraper.Infrastructure.Contexts.Config
{
    public sealed class IssueEntityConfiguration : BaseEntityConfiguration<IssueEntity>
    {
        protected override void ConfigureModel(EntityTypeBuilder<IssueEntity> builder)
        {
            builder.ToTable("issue_entity").HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever().IsRequired();
            builder.Property(p => p.NexusModsGameId).HasColumnName("game_id").IsRequired();
            builder.Property(p => p.NexusModsModId).HasColumnName("mod_id").IsRequired();
            builder.Property(p => p.GameName).HasColumnName("game_name").IsRequired();
            builder.Property(p => p.ModName).HasColumnName("mod_name").IsRequired();
            builder.Property(p => p.Url).HasColumnName("url").IsRequired();
            builder.Property(p => p.ModVersion).HasColumnName("mod_version").IsRequired();
            builder.Property<uint>("priority_id").IsRequired();
            builder.Property<uint>("status_id").IsRequired();
            builder.Property(p => p.IsPrivate).HasColumnName("is_private").IsRequired();
            builder.Property(p => p.IsClosed).HasColumnName("is_closed").IsRequired();
            builder.Property(p => p.IsDeleted).HasColumnName("is_deleted").IsRequired();
            builder.Property(p => p.TimeOfLastPost).HasColumnName("time_of_last_post").IsRequired();
            builder.HasOne(p => p.Priority).WithMany().HasForeignKey("priority_id");
            builder.HasOne(p => p.Status).WithMany().HasForeignKey("status_id");
            builder.HasOne(p => p.Content).WithOne().HasForeignKey<IssueContentEntity>(p => p.Id).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(p => p.Replies).WithOne().HasForeignKey(p => p.OwnerId).OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(b => b.DomainEvents);
            builder.Metadata.FindNavigation(nameof(IssueEntity.Replies))!.SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}