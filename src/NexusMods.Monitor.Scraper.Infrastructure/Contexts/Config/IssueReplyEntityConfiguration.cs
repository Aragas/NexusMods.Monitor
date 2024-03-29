﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Shared.Infrastructure;

namespace NexusMods.Monitor.Scraper.Infrastructure.Contexts.Config
{
    public sealed class IssueReplyEntityConfiguration : BaseEntityConfiguration<IssueReplyEntity>
    {
        protected override void ConfigureModel(EntityTypeBuilder<IssueReplyEntity> builder)
        {
            builder.ToTable("issue_reply_entity", "scraper").HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever().IsRequired();
            builder.Property(p => p.OwnerId).HasColumnName("owner_id").IsRequired();
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