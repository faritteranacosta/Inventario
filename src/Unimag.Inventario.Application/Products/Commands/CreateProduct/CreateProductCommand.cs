namespace Unimag.Inventario.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(Guid TenantId, string Name, string? Description, decimal Price, int Stock, Guid CategoryId, string? ImageUrl);