using System.Reflection;
using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.Services;

namespace UnitTests.Services;

public class BookServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldThrowConflictException_WhenTitleAlreadyExists()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        await service.CreateAsync("Unique Book Name", "Valid Author Name", "Valid description");

        var action = async () => await service.CreateAsync("Unique Book Name", "Another Author", "Another description");

        await Assert.ThrowsAsync<ConflictException>(action);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowDomainValidationException_WhenDataIsInvalid()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        var action = async () => await service.CreateAsync("short", "tiny", new string('x', 2000));

        var exception = await Assert.ThrowsAsync<DomainValidationException>(action);

        Assert.NotEmpty(exception.Errors);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenBookDoesNotExist()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        var action = async () => await service.GetByIdAsync(999);

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenBookDoesNotExist()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        var action = async () => await service.UpdateAsync(1, "Updated Book Name", "Updated Author Name", "Updated description");

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowConflictException_WhenTitleAlreadyExistsInAnotherBook()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        var firstBook = await service.CreateAsync("First Book Title", "First Author Name", "First description");
        var secondBook = await service.CreateAsync("Second Book Name", "Second Author N", "Second description");

        var action = async () => await service.UpdateAsync(secondBook.Id, firstBook.Title, "New Author Name", "New description");

        await Assert.ThrowsAsync<ConflictException>(action);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenBookDoesNotExist()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        var action = async () => await service.DeleteAsync(1000);

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldThrowDomainValidationException_WhenPaginationIsInvalid()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        var invalidPageAction = async () => await service.GetPagedAsync(0, 10);
        var invalidPageSizeAction = async () => await service.GetPagedAsync(1, 0);

        await Assert.ThrowsAsync<DomainValidationException>(invalidPageAction);
        await Assert.ThrowsAsync<DomainValidationException>(invalidPageSizeAction);
    }

    [Fact]
    public async Task CreateAndGetPagedAsync_ShouldReturnCreatedBooks()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        await service.CreateAsync("Book Number One", "Author Number 1", "Description one");
        await service.CreateAsync("Book Number Two", "Author Number 2", "Description two");

        var (items, totalCount) = await service.GetPagedAsync(1, 10);

        Assert.Equal(2, totalCount);
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddBookCreatedDomainEvent()
    {
        var repository = new FakeBookRepository();
        var service = new BookService(repository);

        var createdBook = await service.CreateAsync("Book Number Three", "Author Number 3", "Description three");

        var domainEvent = createdBook.DomainEvents.OfType<BookCreatedDomainEvent>().SingleOrDefault();

        Assert.NotNull(domainEvent);
        Assert.Equal(createdBook.Id, domainEvent!.BookId);
    }

    private sealed class FakeBookRepository : IBookRepository
    {
        private readonly List<Book> _books = [];
        private long _currentId = 1;

        public Task InsertAsync(Book entity)
        {
            SetId(entity, _currentId++);
            entity.SetLastAction();
            _books.Add(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(long id)
        {
            var existing = _books.FirstOrDefault(book => book.Id == id);
            if (existing is not null)
                _books.Remove(existing);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(Book entity)
        {
            entity.SetLastAction();
            return Task.CompletedTask;
        }

        public Task<Book?> GetByIdAsync(long id)
            => Task.FromResult(_books.FirstOrDefault(book => book.Id == id));

        public Task<IEnumerable<Book>> GetAllAsync()
            => Task.FromResult<IEnumerable<Book>>(_books.ToArray());

        public Task<(IReadOnlyCollection<Book> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = _books.Count;
            var items = _books
                .OrderBy(book => book.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArray();

            return Task.FromResult<(IReadOnlyCollection<Book> Items, int TotalCount)>((items, totalCount));
        }

        public Task<bool> ExistsByTitleAsync(string title, long? excludeId = null)
        {
            var normalized = title.Trim().ToLower();
            var query = _books.Where(book => book.Title.ToLower() == normalized);

            if (excludeId.HasValue)
                query = query.Where(book => book.Id != excludeId.Value);

            return Task.FromResult(query.Any());
        }

        private static void SetId(Book entity, long id)
        {
            var property = typeof(Book).GetProperty(nameof(Book.Id), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            property?.SetValue(entity, id);
        }
    }
}
