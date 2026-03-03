using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings;

public abstract class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Entity<TEntity>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("Id");

        builder.Property(p => p.CreatedDate).IsRequired().HasColumnName("CreatedDate").HasColumnType("DATETIME2");

        builder.Property(p => p.LastUpdatedDate).IsRequired(false).HasColumnName("LastUpdatedDate").HasColumnType("DATETIME2");

        builder.Ignore(p => p.ClassLevelCascadeMode);

        builder.Ignore(p => p.RuleLevelCascadeMode);

        builder.Ignore(p => p.CascadeMode);

        builder.Ignore(p => p.ValidationResult);

        builder.Ignore(p => p.DomainEvents);
    }
}