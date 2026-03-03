using Domain.Entities;

namespace Domain.Events;

public class BookCreatedDomainEvent : DomainEvent
{
    public BookCreatedDomainEvent(Book book)
    {
        Book = book;
    }

    public Book Book { get; }
    public long BookId => Book.Id;
    public string Title => Book.Title;
    public string Author => Book.Author;
}
