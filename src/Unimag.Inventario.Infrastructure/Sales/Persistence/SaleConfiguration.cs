using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unimag.Inventario.Domain.Sales;

namespace Unimag.Inventario.Infrastructure.Sales.Persistence;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TenantId).IsRequired();

        builder.Property(s => s.SaleDateUtc).IsRequired();

        builder.Property(s => s.IdempotencyKey).HasMaxLength(200);
        builder.Property(s => s.IdempotencyPayloadHash).HasMaxLength(64);
        builder.HasIndex(s => new { s.TenantId, s.IdempotencyKey }).IsUnique();

        builder.OwnsMany(s => s.Items, item =>
        {
            item.Property(i => i.ProductId).IsRequired();
            item.Property(i => i.Quantity).IsRequired();
            item.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
        });
    }
}