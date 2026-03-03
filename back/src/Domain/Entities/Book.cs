using FluentValidation;

namespace Domain.Entities;

public class Book : Entity<Book>
{
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    protected Book()
    {
        ConfigureValidation();
    }

    public Book(string title, string author, string description)
    {
        ConfigureValidation();
        Title = title;
        Author = author;
        Description = description;
    }

    public void Update(string title, string author, string description)
    {
        Title = title;
        Author = author;
        Description = description;
    }

    public override bool IsValid()
    {
        ValidationResult = Validate(this);

        return ValidationResult.IsValid;
    }

    private void ConfigureValidation()
    {
        RuleFor(book => book.Title)
            .NotEmpty().WithMessage("Título é obrigatório.")
            .Length(10, 100).WithMessage("Título deve ter entre 10 e 100 caracteres.");

        RuleFor(book => book.Author)
            .NotEmpty().WithMessage("Autor é obrigatório.")
            .Length(10, 100).WithMessage("Autor deve ter entre 10 e 100 caracteres.");

        RuleFor(book => book.Description)
            .NotEmpty().WithMessage("Descrição é obrigatória.")
            .MaximumLength(1024).WithMessage("Descrição deve ter no máximo 1024 caracteres.");
    }
}
