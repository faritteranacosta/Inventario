using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unimag.Inventario.Domain.InventoryMovements;

namespace Unimag.Inventario.Infrastructure.InventoryMovements.Persistence;

public sealed class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.TenantId).IsRequired();
        builder.Property(m => m.ProductId).IsRequired();
        builder.Property(m => m.Type).IsRequired();
        builder.Property(m => m.Quantity).IsRequired();
        builder.Property(m => m.StockBefore).IsRequired();
        builder.Property(m => m.StockAfter).IsRequired();
        builder.Property(m => m.Reason).IsRequired().HasMaxLength(500);
        builder.Property(m => m.ReferenceType).HasMaxLength(100);
        builder.Property(m => m.CreatedAtUtc).IsRequired();
        builder.HasIndex(m => new { m.TenantId, m.ProductId, m.CreatedAtUtc });
    }
}
