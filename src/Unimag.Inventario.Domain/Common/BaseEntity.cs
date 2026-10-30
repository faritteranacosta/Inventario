namespace Unimag.Inventario.Domain.Common;

/// <summary>
/// Base type for every aggregate/entity in the Domain layer.
/// The Domain layer has zero dependencies on Application, Infrastructure or Api.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected init; }
}
