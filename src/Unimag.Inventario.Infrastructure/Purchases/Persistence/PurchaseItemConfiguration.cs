using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unimag.Inventario.Domain.Purchases;

namespace Unimag.Inventario.Infrastructure.Purchases.Persistence;

public sealed class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.PurchaseId).IsRequired();
        builder.Property(i => i.ProductId).IsRequired();
        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.UnitCost).HasColumnType("decimal(18,2)").IsRequired();
        builder.Ignore(i => i.Total);
    }
}
