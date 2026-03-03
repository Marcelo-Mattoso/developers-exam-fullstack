using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(SqlDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyCollection<Book> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        var query = DbSet.AsNoTracking().OrderBy(book => book.Id);
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> ExistsByTitleAsync(string title, long? excludeId = null)
    {
        var normalizedTitle = title.Trim().ToLower();

        var query = DbSet.AsNoTracking().Where(book => book.Title.ToLower() == normalizedTitle);

        if (excludeId.HasValue)
            query = query.Where(book => book.Id != excludeId.Value);

        return await query.AnyAsync();
    }
}
