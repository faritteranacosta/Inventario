namespace Unimag.Inventario.Api.Sales.Requests;

public sealed record CreateSaleItemRequest(Guid ProductId, int Quantity);

public sealed record CreateSaleRequest(List<CreateSaleItemRequest> Items);