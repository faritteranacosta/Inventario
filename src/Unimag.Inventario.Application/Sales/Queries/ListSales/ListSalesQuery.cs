namespace Unimag.Inventario.Application.Sales.Queries.ListSales;

public sealed record ListSalesQuery(DateTime? FromUtc = null, DateTime? ToUtc = null);