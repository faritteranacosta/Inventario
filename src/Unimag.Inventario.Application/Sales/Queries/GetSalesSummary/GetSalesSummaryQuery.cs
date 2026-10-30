namespace Unimag.Inventario.Application.Sales.Queries.GetSalesSummary;
public sealed record GetSalesSummaryQuery(DateTime FromUtc, DateTime ToUtc, string GroupBy = "day");