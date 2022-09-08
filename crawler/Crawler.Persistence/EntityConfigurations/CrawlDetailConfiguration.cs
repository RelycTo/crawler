﻿using Crawler.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crawler.Persistence.EntityConfigurations;

public class CrawlDetailConfiguration : IEntityTypeConfiguration<CrawlDetail>
{
    public void Configure(EntityTypeBuilder<CrawlDetail> builder)
    {
        builder
            .HasKey(p => p.Id);
        builder.Property(p => p.CreatedUtc)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(p => p.Address)
            .IsRequired();
        builder.Property(p => p.SourceType)
            .IsRequired();
        builder.Property(p => p.CrawlInfoId)
            .IsRequired();
    }
}