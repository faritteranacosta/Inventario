namespace Unimag.Inventario.Application.Sales.Commands.CreateSale;

public sealed record CreateSaleCommand(IReadOnlyList<(Guid ProductId, int Quantity)> Items);