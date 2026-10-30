namespace Unimag.Inventario.Application.Categories.Dtos;

public sealed record CategoryResponse(Guid Id, string Name, string? Description, DateTime CreatedAtUtc);