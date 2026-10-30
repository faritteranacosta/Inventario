namespace Unimag.Inventario.Application.Samples.Dtos;

/// <summary>
/// Shape returned to driving adapters (Api). Never leak Domain entities
/// across the Application boundary — map to/from DTOs like this one.
/// </summary>
public sealed record SampleResponse(Guid Id, string Name, string? Description, DateTime CreatedAtUtc);
