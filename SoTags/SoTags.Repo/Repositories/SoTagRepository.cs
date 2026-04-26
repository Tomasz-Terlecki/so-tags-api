using Microsoft.EntityFrameworkCore;
using SoTags.Domain.Enums;
using SoTags.Domain.Interfaces.Repositories;
using SoTags.Domain.Models;

namespace SoTags.Repo.Repositories;

public class SoTagRepository : ISoTagRepository
{
    private readonly SoTagDbContext _context;

    public SoTagRepository(SoTagDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SoTag>> GetPagedAsync(int pageNumber, int pageSize, SortBy sortBy = SortBy.None, SortDirection sortDirection = SortDirection.Ascending)
    {
        if (pageNumber < 1)
            pageNumber = 1;
        if (pageSize < 1)
            pageSize = 10;

        var skip = (pageNumber - 1) * pageSize;

        IQueryable<SoTag> query = _context.SoTags;

        // Apply sorting
        query = sortBy switch
        {
            SortBy.None => query,
            SortBy.Name => sortDirection == SortDirection.Ascending
                ? query.OrderBy(t => t.Name)
                : query.OrderByDescending(t => t.Name),
            SortBy.Share => sortDirection == SortDirection.Ascending
                ? query.OrderBy(t => t.Count)
                : query.OrderByDescending(t => t.Count),
            _ => throw new ArgumentException("Invalid sort by value", nameof(sortBy))
        };

        return await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.SoTags.CountAsync();
    }

    public async Task<SoTag?> GetByIdAsync(Guid id)
    {
        return await _context.SoTags.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddAsync(SoTag soTag)
    {
        await _context.SoTags.AddAsync(soTag);
    }

    public async Task RemoveAsync(Guid id)
    {
        var soTag = await GetByIdAsync(id);
        if (soTag is not null)
        {
            _context.SoTags.Remove(soTag);
        }
    }

    public async Task RemoveAllAsync()
    {
        var tags = await _context.SoTags.ToListAsync();
        if (tags.Count > 0)
        {
            _context.SoTags.RemoveRange(tags);
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
