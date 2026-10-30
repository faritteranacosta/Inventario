namespace Unimag.Inventario.Application.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(Guid Id, string Name, string? Description, decimal Price, int Stock, Guid CategoryId);