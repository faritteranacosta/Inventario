using Unimag.Inventario.Application.Products.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Products.Contracts;

namespace Unimag.Inventario.Application.Products.Queries.ListProducts;

public sealed class ListProductsUseCase : IUseCase<ListProductsQuery, IReadOnlyList<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public ListProductsUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductResponse>> ExecuteAsync(ListProductsQuery input, CancellationToken cancellationToken = default)
    {
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

        return products.Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Stock, p.CategoryId, p.ImageUrl, p.CreatedAtUtc)).ToList();
    }
}