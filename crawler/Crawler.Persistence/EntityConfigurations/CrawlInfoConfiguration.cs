using Crawler.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crawler.Persistence.EntityConfigurations;

public class CrawlInfoConfiguration : IEntityTypeConfiguration<CrawlInfo>
{
    public void Configure(EntityTypeBuilder<CrawlInfo> builder)
    {
        builder
            .HasKey(p => p.Id);
        builder.Property(p => p.CreatedUtc)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(p => p.TargetUrl)
            .IsRequired();
        builder.Property(p => p.UpdatedUtc)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("GETUTCDATE()");
    }
}