using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Samples.Exceptions;

namespace Unimag.Inventario.Domain.Samples;

/// <summary>
/// Sample aggregate root. This is the reference "domain" folder: every
/// business domain in this solution gets its own folder here, mirrored by a
/// same-named folder in Application, Infrastructure and Api.
/// </summary>
public sealed class Sample : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Sample()
    {
        // Required by EF Core materialization.
    }

    private Sample(Guid id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method: this is where business invariants are enforced,
    /// instead of exposing public setters (rich domain model).
    /// </summary>
    public static Sample Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new SampleNameRequiredException();
        }

        return new Sample(Guid.NewGuid(), name.Trim(), description?.Trim());
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new SampleNameRequiredException();
        }

        Name = name.Trim();
    }
}
