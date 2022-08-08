using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace DataAccess.Repositories;

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
        return await _context.CrawlInfo.FindAsync(id, token);
    }

    public async Task UpdateAsync(CrawlInfo entity, CancellationToken token)
    {
        var row = await _context.CrawlInfo.FindAsync(entity.Id, token);
        if (row == null)
            throw new InvalidOperationException($"{nameof(CrawlInfo)} entity with id: {entity.Id} is not found");

        row = entity;
        await _context.SaveChangesAsync(token);
    }

    public async Task<IEnumerable<CrawlInfo>> GetAllAsync(CancellationToken token) =>
        await _context.CrawlInfo.ToListAsync(token);
}