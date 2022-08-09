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

        builder.Entity<CrawlInfo>().Property(p => p.CreatedUtc)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Entity<CrawlInfo>().Property(p => p.UpdatedUtc)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Entity<CrawlDetail>().Property(p => p.CreatedUtc)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("GETUTCDATE()");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}
