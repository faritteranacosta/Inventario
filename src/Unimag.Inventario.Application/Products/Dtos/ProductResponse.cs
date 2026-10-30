namespace Unimag.Inventario.Application.Products.Dtos;

public sealed record ProductResponse(Guid Id, string Name, string? Description, decimal Price, int Stock, Guid CategoryId, string? ImageUrl, DateTime CreatedAtUtc);