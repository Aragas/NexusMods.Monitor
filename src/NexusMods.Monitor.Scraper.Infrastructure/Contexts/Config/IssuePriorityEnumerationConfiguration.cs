using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;

namespace NexusMods.Monitor.Scraper.Infrastructure.Contexts.Config
{
    public sealed class IssuePriorityEnumerationConfiguration : IEntityTypeConfiguration<IssuePriorityEnumeration>
    {
        public void Configure(EntityTypeBuilder<IssuePriorityEnumeration> builder)
        {
            builder.ToTable("issue_priority_enumeration").HasKey(p => p.Id);
            builder.Property(p => p.Id).HasDefaultValue(1).ValueGeneratedNever().HasColumnName("id").IsRequired();
            builder.Property(p => p.Name).HasColumnName("name").IsRequired();
        }
    }
}