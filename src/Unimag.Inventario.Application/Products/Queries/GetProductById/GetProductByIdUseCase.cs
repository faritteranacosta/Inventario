using Unimag.Inventario.Application.Products.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Domain.Products.Exceptions;

namespace Unimag.Inventario.Application.Products.Queries.GetProductById;

public sealed class GetProductByIdUseCase : IUseCase<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponse> ExecuteAsync(GetProductByIdQuery input, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new ProductNotFoundException(input.Id);

        return new ProductResponse(product.Id, product.Name, product.Description, product.Price, product.Stock, product.CategoryId, product.ImageUrl, product.CreatedAtUtc);
    }
}