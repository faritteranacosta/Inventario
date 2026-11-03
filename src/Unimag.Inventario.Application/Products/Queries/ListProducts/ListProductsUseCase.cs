using Unimag.Inventario.Application.Products.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Application.Common.Dtos;
using Unimag.Inventario.Domain.Common.Exceptions;

namespace Unimag.Inventario.Application.Products.Queries.ListProducts;

public sealed class ListProductsUseCase : IUseCase<ListProductsQuery, PagedResponse<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public ListProductsUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResponse<ProductResponse>> ExecuteAsync(ListProductsQuery input, CancellationToken cancellationToken = default)
    {
        if (input.Page < 1 || input.PageSize is < 1 or > 100)
        {
            throw new InvalidPaginationException();
        }

        var result = await _productRepository.GetPageAsync(
            input.CategoryId, input.Search, (input.Page - 1) * input.PageSize, input.PageSize,
            cancellationToken);
        var items = result.Items
            .Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Stock, p.CategoryId, p.ImageUrl, p.MinimumStock, p.Barcode, p.CreatedAtUtc))
            .ToList();

        return new PagedResponse<ProductResponse>(
            items,
            input.Page,
            input.PageSize,
            result.TotalItems,
            (int)Math.Ceiling(result.TotalItems / (double)input.PageSize));
    }
}