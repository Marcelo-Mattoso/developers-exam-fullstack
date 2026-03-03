using Domain.Entities;

namespace UnitTests.Domain;

public class BookTests
{
    [Fact]
    public void IsValid_ShouldReturnTrue_WhenBookHasValidFields()
    {
        var book = new Book(
            "A Very Good Book",
            "Great Author Name",
            "Short description");

        var isValid = book.IsValid();

        Assert.True(isValid);
        Assert.Empty(book.ValidationResult.Errors);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenTitleIsTooShort()
    {
        var book = new Book(
            "Short",
            "Great Author Name",
            "Short description");

        var isValid = book.IsValid();

        Assert.False(isValid);
        Assert.Contains(book.ValidationResult.Errors, error => error.ErrorMessage == "Título deve ter entre 10 e 100 caracteres.");
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenAuthorIsTooShort()
    {
        var book = new Book(
            "A Very Good Book",
            "Author",
            "Short description");

        var isValid = book.IsValid();

        Assert.False(isValid);
        Assert.Contains(book.ValidationResult.Errors, error => error.ErrorMessage == "Autor deve ter entre 10 e 100 caracteres.");
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenDescriptionExceedsMaxLength()
    {
        var description = new string('x', 1025);
        var book = new Book(
            "A Very Good Book",
            "Great Author Name",
            description);

        var isValid = book.IsValid();

        Assert.False(isValid);
        Assert.Contains(book.ValidationResult.Errors, error => error.ErrorMessage == "Descrição deve ter no máximo 1024 caracteres.");
    }
}
