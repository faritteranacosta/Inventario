using System.Globalization;
using System.Text;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Products.Contracts;

namespace Unimag.Inventario.Application.Reports.Queries.GenerateInventoryCsv;

public sealed class GenerateInventoryCsvUseCase : IUseCase<GenerateInventoryCsvQuery, byte[]>
{
    private readonly IProductRepository _repository;

    public GenerateInventoryCsvUseCase(IProductRepository repository) => _repository = repository;

    public async Task<byte[]> ExecuteAsync(
        GenerateInventoryCsvQuery input,
        CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllAsync(cancellationToken);
        var builder = new StringBuilder();
        builder.AppendLine("productId,name,stock,minimumStock,barcode,price");
        foreach (var product in products)
        {
            builder.AppendLine(string.Join(",",
                Escape(product.Id.ToString()),
                Escape(product.Name),
                Escape(product.Stock.ToString(CultureInfo.InvariantCulture)),
                Escape(product.MinimumStock?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
                Escape(product.Barcode ?? string.Empty),
                Escape(product.Price.ToString(CultureInfo.InvariantCulture))));
        }

        return Encoding.UTF8.GetBytes(builder.ToString());
    }

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
