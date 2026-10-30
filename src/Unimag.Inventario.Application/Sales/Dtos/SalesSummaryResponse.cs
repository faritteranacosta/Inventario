namespace Unimag.Inventario.Application.Sales.Dtos;

public sealed record SalesSummaryItem(DateTime Period, decimal Total, int Count);

public sealed record SalesSummaryResponse(IReadOnlyList<SalesSummaryItem> Items, decimal GrandTotal, int TotalCount);