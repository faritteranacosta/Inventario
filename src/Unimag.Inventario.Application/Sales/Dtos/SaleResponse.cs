namespace Unimag.Inventario.Application.Sales.Dtos;

public sealed record SaleItemResponse(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal Total);

public sealed record SaleResponse(Guid Id, string TenantName, DateTime SaleDateUtc, IReadOnlyList<SaleItemResponse> Items, decimal Total);