using Crawler.App.DTOs;
using Crawler.App.Services.Repositories;
using Crawler.DataAccess.Models;
using Crawler.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Crawler.DataAccess.Repositories;

public class CrawlInfoRepository : ICrawlInfoRepository
{
    private readonly CrawlerDbContext _context;

    public CrawlInfoRepository(CrawlerDbContext context)
    {
        _context = context;
    }

    public async Task<int> InsertAsync(CrawlInfoDto dto, CancellationToken token)
    {
        var entity = new CrawlInfo
        {
            Status = dto.Status,
            TargetUrl = dto.Url
        };
        await _context.CrawlInfo.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);
        return entity.Id;
    }

    public async Task<CrawlInfo> GetAsync(int id, CancellationToken token)
    {
        return await _context.CrawlInfo.FindAsync(id);
    }

    public async Task UpdateAsync(CrawlInfo entity, CancellationToken token)
    {
        await _context.SaveChangesAsync(token);
    }

    public async Task<IEnumerable<CrawlInfo>> GetAllAsync(CancellationToken token) =>
        await _context.CrawlInfo.ToListAsync(token);
}