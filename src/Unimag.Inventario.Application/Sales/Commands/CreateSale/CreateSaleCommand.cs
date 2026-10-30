namespace Unimag.Inventario.Application.Sales.Commands.CreateSale;

public sealed record CreateSaleCommand(Guid TenantId, IReadOnlyList<(Guid ProductId, int Quantity)> Items);