using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unimag.Inventario.Domain.Tenants;

namespace Unimag.Inventario.Infrastructure.Tenants.Persistence;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Status).IsRequired();
        builder.Property(t => t.CreatedAtUtc).IsRequired();
    }
}