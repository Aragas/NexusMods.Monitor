using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;

namespace NexusMods.Monitor.Scraper.Infrastructure.Contexts.Config
{
    public sealed class CommentEntityConfiguration : IEntityTypeConfiguration<CommentEntity>
    {
        public void Configure(EntityTypeBuilder<CommentEntity> builder)
        {
            builder.ToTable("comment_entity").HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever().IsRequired();
            builder.Property(p => p.NexusModsGameId).HasColumnName("game_id").IsRequired();
            builder.Property(p => p.NexusModsModId).HasColumnName("mod_id").IsRequired();
            builder.Property(p => p.Url).HasColumnName("url").IsRequired();
            builder.Property(p => p.Author).HasColumnName("author").IsRequired();
            builder.Property(p => p.AuthorUrl).HasColumnName("author_url").IsRequired();
            builder.Property(p => p.AvatarUrl).HasColumnName("avatar_url").IsRequired();
            builder.Property(p => p.Content).HasColumnName("content").IsRequired();
            builder.Property(p => p.IsSticky).HasColumnName("is_sticky").IsRequired();
            builder.Property(p => p.IsLocked).HasColumnName("is_locked").IsRequired();
            builder.Property(p => p.IsDeleted).HasColumnName("is_deleted").IsRequired();
            builder.Property(p => p.TimeOfPost).HasColumnName("time_of_post").IsRequired();
            builder.HasMany(p => p.Replies).WithOne().HasForeignKey(p => p.OwnerId).OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(b => b.DomainEvents);
            builder.Metadata.FindNavigation(nameof(CommentEntity.Replies)).SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}