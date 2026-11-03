using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unimag.Inventario.Domain.Suppliers;

namespace Unimag.Inventario.Infrastructure.Suppliers.Persistence;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.TenantId).IsRequired();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Email).HasMaxLength(320);
        builder.Property(s => s.Phone).HasMaxLength(50);
        builder.Property(s => s.IsActive).IsRequired();
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.HasIndex(s => new { s.TenantId, s.Name });
    }
}
