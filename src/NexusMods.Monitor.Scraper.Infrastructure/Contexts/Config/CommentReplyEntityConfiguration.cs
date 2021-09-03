using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Shared.Infrastructure;

namespace NexusMods.Monitor.Scraper.Infrastructure.Contexts.Config
{
    public sealed class CommentReplyEntityConfiguration : BaseEntityConfiguration<CommentReplyEntity>
    {
        protected override void ConfigureModel(EntityTypeBuilder<CommentReplyEntity> builder)
        {
            builder.ToTable("comment_reply_entity").HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever().IsRequired();
            builder.Property(p => p.OwnerId).HasColumnName("owner_id").IsRequired();
            builder.Property(p => p.Url).HasColumnName("url").IsRequired();
            builder.Property(p => p.Author).HasColumnName("author").IsRequired();
            builder.Property(p => p.AuthorUrl).HasColumnName("author_url").IsRequired();
            builder.Property(p => p.AvatarUrl).HasColumnName("avatar_url").IsRequired();
            builder.Property(p => p.Content).HasColumnName("content").IsRequired();
            builder.Property(p => p.IsDeleted).HasColumnName("is_deleted").IsRequired();
            builder.Property(p => p.TimeOfPost).HasColumnName("time_of_post").IsRequired();
            builder.Ignore(b => b.DomainEvents);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}