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

        IReadOnlyList<Domain.Products.Product> products;
        if (input.CategoryId.HasValue)
        {
            products = await _productRepository.GetByCategoryAsync(input.CategoryId.Value, cancellationToken);
        }
        else
        {
            products = await _productRepository.GetAllAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            var term = input.Search.Trim().ToLowerInvariant();
            products = products.Where(p => p.Name.ToLowerInvariant().Contains(term)).ToList();
        }

        var totalItems = products.Count;
        var items = products
            .OrderBy(p => p.CreatedAtUtc)
            .ThenBy(p => p.Id)
            .Skip((input.Page - 1) * input.PageSize)
            .Take(input.PageSize)
            .Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Stock, p.CategoryId, p.ImageUrl, p.MinimumStock, p.Barcode, p.CreatedAtUtc))
            .ToList();

        return new PagedResponse<ProductResponse>(
            items,
            input.Page,
            input.PageSize,
            totalItems,
            (int)Math.Ceiling(totalItems / (double)input.PageSize));
    }
}