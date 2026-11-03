using Unimag.Inventario.Application.Reports.Queries.GenerateSalesCsv;

namespace Unimag.Inventario.Api.Reports.Mappers;

public static class ReportMapper
{
    public static GenerateSalesCsvQuery ToSalesCsvQuery(DateTime? from, DateTime? to) =>
        new(from, to);
}
