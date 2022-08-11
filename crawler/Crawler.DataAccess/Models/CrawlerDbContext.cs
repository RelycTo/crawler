﻿using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Crawler.DataAccess.Models;

public class CrawlerDbContext: DbContext
{
    public CrawlerDbContext(DbContextOptions<CrawlerDbContext> options) : base(options) { }

    public virtual DbSet<CrawlInfo> CrawlInfo { get; set; }
    public virtual DbSet<CrawlDetail> CrawlDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}