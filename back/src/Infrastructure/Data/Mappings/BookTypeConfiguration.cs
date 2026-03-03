using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings;

public class BookTypeConfiguration : EntityTypeConfiguration<Book>
{
    public override void Configure(EntityTypeBuilder<Book> builder)
    {
        base.Configure(builder);

        builder.ToTable("Book");

        builder.Property(book => book.Title)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("Title");

        builder.Property(book => book.Author)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("Author");

        builder.Property(book => book.Description)
            .IsRequired()
            .HasMaxLength(1024)
            .HasColumnName("Description");

        builder.HasIndex(book => book.Title)
            .IsUnique();

        builder.HasData(
            new
            {
                Id = 1L,
                Title = "Domain-Driven Design Book",
                Author = "Eric Evans Author",
                Description = "Classic reference for strategic and tactical domain-driven design.",
                CreatedDate = new DateTime(2026, 3, 3, 0, 0, 0),
                LastUpdatedDate = (DateTime?)null
            },
            new
            {
                Id = 2L,
                Title = "Clean Architecture Guide",
                Author = "Robert Martin Author",
                Description = "Practical guidance on layered architecture and maintainable software systems.",
                CreatedDate = new DateTime(2026, 3, 3, 0, 0, 0),
                LastUpdatedDate = (DateTime?)null
            });
    }
}
