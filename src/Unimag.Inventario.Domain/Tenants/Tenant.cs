using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Tenants.Exceptions;

namespace Unimag.Inventario.Domain.Tenants;

public sealed class Tenant : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    private Tenant() { }

    private Tenant(Guid id, string name)
    {
        Id = id;
        Name = name;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Tenant Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new TenantNameRequiredException();
        return new Tenant(Guid.NewGuid(), name.Trim());
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new TenantNameRequiredException();
        Name = name.Trim();
    }
}