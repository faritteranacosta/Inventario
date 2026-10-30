using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unimag.Inventario.Domain.Users;

namespace Unimag.Inventario.Infrastructure.Users.Persistence;

public sealed class TenantMembershipConfiguration : IEntityTypeConfiguration<TenantMembership>
{
    public void Configure(EntityTypeBuilder<TenantMembership> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.UserId).IsRequired();
        builder.Property(m => m.TenantId).IsRequired();
        builder.Property(m => m.Role).IsRequired();
        builder.Property(m => m.Status).IsRequired();
        builder.Property(m => m.CreatedAtUtc).IsRequired();
        builder.HasIndex(m => new { m.UserId, m.TenantId }).IsUnique();
    }
}
