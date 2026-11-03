using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Suppliers.Exceptions;

namespace Unimag.Inventario.Domain.Suppliers;

public sealed class Supplier : BaseEntity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Supplier()
    {
    }

    private Supplier(Guid tenantId, string name, string? email, string? phone)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        Name = name.Trim();
        Email = email?.Trim();
        Phone = phone?.Trim();
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Supplier Create(Guid tenantId, string name, string? email, string? phone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new SupplierNameRequiredException();
        }

        return new Supplier(tenantId, name, email, phone);
    }

    public void UpdateDetails(string name, string? email, string? phone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new SupplierNameRequiredException();
        }

        Name = name.Trim();
        Email = email?.Trim();
        Phone = phone?.Trim();
    }

    public void Deactivate() => IsActive = false;
}
