using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unimag.Inventario.Domain.Products;

namespace Unimag.Inventario.Infrastructure.Products.Persistence;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.TenantId).IsRequired();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.Stock)
            .IsRequired();

        builder.Property(p => p.CategoryId)
            .IsRequired();

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();

        builder.HasOne<Unimag.Inventario.Domain.Categories.Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}