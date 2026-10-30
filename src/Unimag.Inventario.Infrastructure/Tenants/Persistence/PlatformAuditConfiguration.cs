using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unimag.Inventario.Domain.Tenants;

namespace Unimag.Inventario.Infrastructure.Tenants.Persistence;

public sealed class PlatformAuditConfiguration : IEntityTypeConfiguration<PlatformAuditEvent>
{
    public void Configure(EntityTypeBuilder<PlatformAuditEvent> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Action).IsRequired().HasMaxLength(100);
        builder.Property(e => e.ResourceType).IsRequired().HasMaxLength(100);
        builder.Property(e => e.CreatedAtUtc).IsRequired();
        builder.HasIndex(e => e.CreatedAtUtc);
    }
}
