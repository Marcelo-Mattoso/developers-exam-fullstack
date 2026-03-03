using Domain.Entities;

namespace Domain.Interfaces;

public interface IBookService
{
    Task<(IReadOnlyCollection<Book> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);

    Task<Book> GetByIdAsync(long id);

    Task<Book> CreateAsync(string title, string author, string description);

    Task<Book> UpdateAsync(long id, string title, string author, string description);

    Task DeleteAsync(long id);
}
