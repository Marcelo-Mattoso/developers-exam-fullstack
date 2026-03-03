using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Infrastructure.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<(IReadOnlyCollection<Book> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        if (page <= 0)
            throw new DomainValidationException(["O parâmetro page deve ser maior que zero."]);

        if (pageSize <= 0)
            throw new DomainValidationException(["O parâmetro pageSize deve ser maior que zero."]);

        return await _bookRepository.GetPagedAsync(page, pageSize);
    }

    public async Task<Book> GetByIdAsync(long id)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book is null)
            throw new NotFoundException("Livro não encontrado.");

        return book;
    }

    public async Task<Book> CreateAsync(string title, string author, string description)
    {
        var normalizedTitle = Normalize(title);
        var normalizedAuthor = Normalize(author);
        var normalizedDescription = Normalize(description);

        if (await _bookRepository.ExistsByTitleAsync(normalizedTitle))
            throw new ConflictException("Já existe um livro com este título.");

        var book = new Book(normalizedTitle, normalizedAuthor, normalizedDescription);

        if (!book.IsValid())
            throw new DomainValidationException(book.ValidationResult.Errors.Select(error => error.ErrorMessage));

        book.DomainEvents.Add(new BookCreatedDomainEvent(book));

        await _bookRepository.InsertAsync(book);

        return book;
    }

    public async Task<Book> UpdateAsync(long id, string title, string author, string description)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book is null)
            throw new NotFoundException("Livro não encontrado.");

        var normalizedTitle = Normalize(title);
        var normalizedAuthor = Normalize(author);
        var normalizedDescription = Normalize(description);

        if (await _bookRepository.ExistsByTitleAsync(normalizedTitle, id))
            throw new ConflictException("Já existe um livro com este título.");

        book.Update(normalizedTitle, normalizedAuthor, normalizedDescription);

        if (!book.IsValid())
            throw new DomainValidationException(book.ValidationResult.Errors.Select(error => error.ErrorMessage));

        await _bookRepository.UpdateAsync(book);

        return book;
    }

    public async Task DeleteAsync(long id)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book is null)
            throw new NotFoundException("Livro não encontrado.");

        await _bookRepository.DeleteAsync(id);
    }

    private static string Normalize(string value)
        => (value ?? string.Empty).Trim();
}
