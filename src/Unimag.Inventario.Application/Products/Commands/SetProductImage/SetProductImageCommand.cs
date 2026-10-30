namespace Unimag.Inventario.Application.Products.Commands.SetProductImage;

public sealed record SetProductImageCommand(Guid Id, string? ImageUrl);