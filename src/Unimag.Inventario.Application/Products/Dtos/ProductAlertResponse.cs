namespace Unimag.Inventario.Application.Products.Dtos;

public sealed record ProductAlertResponse(
    Guid ProductId,
    string Name,
    int Stock,
    int? MinimumStock,
    string Kind);
