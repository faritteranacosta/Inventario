using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Products.Dtos;
using Unimag.Inventario.Domain.Products.Contracts;

namespace Unimag.Inventario.Application.Products.Queries.ListProductAlerts;

public sealed class ListProductAlertsUseCase
    : IUseCase<ListProductAlertsQuery, IReadOnlyList<ProductAlertResponse>>
{
    private readonly IProductRepository _repository;

    public ListProductAlertsUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProductAlertResponse>> ExecuteAsync(
        ListProductAlertsQuery input,
        CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetLowStockAsync(cancellationToken);
        var kind = input.Kind?.Trim().ToUpperInvariant();

        return products
            .Select(product => new ProductAlertResponse(
                product.Id,
                product.Name,
                product.Stock,
                product.MinimumStock,
                product.Stock == 0 ? "OUT" : "LOW"))
            .Where(alert => kind is null || alert.Kind == kind)
            .ToList();
    }
}
