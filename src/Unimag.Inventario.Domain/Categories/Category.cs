using Unimag.Inventario.Domain.Categories.Exceptions;
using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Categories;

public sealed class Category : BaseEntity
{
    public Guid TenantId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Category()
    {
        // Required by EF Core materialization.
    }

    private Category(Guid id, Guid tenantId, string name, string? description)
    {
        Id = id;
        TenantId = tenantId;
        Name = name;
        Description = description;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Category Create(Guid tenantId, string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new CategoryNameRequiredException();
        }

        return new Category(Guid.NewGuid(), tenantId, name.Trim(), description?.Trim());
    }

    public void Rename(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new CategoryNameRequiredException();
        }

        Name = name.Trim();
        Description = description?.Trim();
    }
}