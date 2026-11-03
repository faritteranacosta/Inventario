namespace Unimag.Inventario.Application.Reports.Queries.GenerateSalesCsv;

public sealed record GenerateSalesCsvQuery(DateTime? FromUtc = null, DateTime? ToUtc = null);
