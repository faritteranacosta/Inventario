using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Tenants.Exceptions;

namespace Unimag.Inventario.Domain.Tenants;

public sealed class Tenant : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? LogoUrl { get; private set; }
    public TenantStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Tenant() { }

    private Tenant(Guid id, string name)
    {
        Id = id;
        Name = name;
        DisplayName = name;
        Status = TenantStatus.Active;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Tenant Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new TenantNameRequiredException();
        return new Tenant(Guid.NewGuid(), name.Trim());
    }

    public void SetBranding(string? displayName, string? logoUrl)
    {
        if (!string.IsNullOrWhiteSpace(displayName)) DisplayName = displayName.Trim();
        if (logoUrl is not null) LogoUrl = string.IsNullOrWhiteSpace(logoUrl) ? null : logoUrl.Trim();
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new TenantNameRequiredException();
        Name = name.Trim();
    }

    public void ChangeStatus(TenantStatus status) => Status = status;
}