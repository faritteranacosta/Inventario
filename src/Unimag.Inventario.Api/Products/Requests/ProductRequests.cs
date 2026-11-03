namespace Unimag.Inventario.Api.Products.Requests;

public sealed record CreateProductRequest(string Name, string? Description, decimal Price, int Stock, Guid CategoryId, string? ImageUrl, int? MinimumStock, string? Barcode);

public sealed record UpdateProductRequest(string Name, string? Description, decimal Price, int Stock, Guid CategoryId, int? MinimumStock, string? Barcode);

public sealed record SetProductImageRequest(string? ImageUrl);