using Domain.Entities;

namespace Domain.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<(IReadOnlyCollection<Book> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);

    Task<bool> ExistsByTitleAsync(string title, long? excludeId = null);
}
