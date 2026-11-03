using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unimag.Inventario.Domain.Purchases;

namespace Unimag.Inventario.Infrastructure.Purchases.Persistence;

public sealed class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.TenantId).IsRequired();
        builder.Property(p => p.Status).IsRequired();
        builder.Property(p => p.ReceivedAtUtc);
        builder.Property(p => p.CreatedByUserId).IsRequired();
        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.HasMany(p => p.Items).WithOne().HasForeignKey(i => i.PurchaseId);
        builder.Ignore(p => p.Total);
    }
}
