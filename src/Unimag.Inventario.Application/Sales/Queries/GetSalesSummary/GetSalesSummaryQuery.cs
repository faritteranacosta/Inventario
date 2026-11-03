namespace Unimag.Inventario.Application.Sales.Queries.GetSalesSummary;
public sealed record GetSalesSummaryQuery(DateTime? FromUtc = null, DateTime? ToUtc = null, string GroupBy = "day");