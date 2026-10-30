namespace Unimag.Inventario.Api.Products.Requests;

public sealed record CreateProductRequest(string Name, string? Description, decimal Price, int Stock, Guid CategoryId, string? ImageUrl);

public sealed record UpdateProductRequest(string Name, string? Description, decimal Price, int Stock, Guid CategoryId);

public sealed record SetProductImageRequest(string? ImageUrl);