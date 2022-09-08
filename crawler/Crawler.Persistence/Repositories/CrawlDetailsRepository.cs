using Crawler.Application.Services.Repositories;
using Crawler.Domain.Models;
using Crawler.Persistence.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace Crawler.Persistence.Repositories;

public class CrawlDetailsRepository : ICrawlDetailsRepository
{
    private readonly CrawlerDbContext _context;

    public CrawlDetailsRepository(CrawlerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CrawlDetail>> GetCrawlDetailsAsync(int crawlId, CancellationToken token) =>
        await _context.CrawlDetails
            .Where(d => d.CrawlInfoId == crawlId)
            .ToListAsync(token);

    public async Task BulkInsertAsync(IEnumerable<CrawlDetail> entities, int batchSize, CancellationToken token)
    {
        var items = entities.ToArray();
        await _context.BulkInsertAsync(items, x => x.BatchSize = batchSize, cancellationToken: token);
    }
}
