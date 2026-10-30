namespace Unimag.Inventario.Application.Sales.Dtos;

public sealed record SaleItemResponse(Guid ProductId, int Quantity, decimal UnitPrice, decimal Total);

public sealed record SaleResponse(Guid Id, DateTime SaleDateUtc, IReadOnlyList<SaleItemResponse> Items, decimal Total);