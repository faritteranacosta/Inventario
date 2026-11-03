using System.Globalization;
using System.Text;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Sales;
using Unimag.Inventario.Domain.Sales.Contracts;

namespace Unimag.Inventario.Application.Reports.Queries.GenerateSalesCsv;

public sealed class GenerateSalesCsvUseCase : IUseCase<GenerateSalesCsvQuery, byte[]>
{
    private readonly ISaleRepository _repository;

    public GenerateSalesCsvUseCase(ISaleRepository repository) => _repository = repository;

    public async Task<byte[]> ExecuteAsync(
        GenerateSalesCsvQuery input,
        CancellationToken cancellationToken = default)
    {
        var sales = input.FromUtc.HasValue && input.ToUtc.HasValue
            ? await _repository.GetByDateRangeAsync(input.FromUtc.Value, input.ToUtc.Value, cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        var builder = new StringBuilder();
        builder.AppendLine("saleId,productId,productName,quantity,unitPrice,total,saleDateUtc");
        foreach (var sale in sales)
        {
            foreach (var item in sale.Items)
            {
                AppendRow(builder,
                    sale.Id.ToString(),
                    item.ProductId.ToString(),
                    item.ProductName,
                    item.Quantity.ToString(CultureInfo.InvariantCulture),
                    item.UnitPrice.ToString(CultureInfo.InvariantCulture),
                    item.Total.ToString(CultureInfo.InvariantCulture),
                    sale.SaleDateUtc.ToString("O", CultureInfo.InvariantCulture));
            }
        }

        return Encoding.UTF8.GetBytes(builder.ToString());
    }

    private static void AppendRow(StringBuilder builder, params string[] values) =>
        builder.AppendLine(string.Join(",", values.Select(Escape)));

    private static string Escape(string value)
    {
        var escaped = value.Replace("\"", "\"\"");
        if (escaped.Length > 0 && escaped[0] is '=' or '+' or '-' or '@')
        {
            escaped = "'" + escaped;
        }

        return $"\"{escaped}\"";
    }
}
