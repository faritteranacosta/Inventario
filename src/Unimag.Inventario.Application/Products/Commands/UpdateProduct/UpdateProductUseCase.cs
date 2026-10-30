using Unimag.Inventario.Application.Products.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Domain.Products.Exceptions;

namespace Unimag.Inventario.Application.Products.Commands.UpdateProduct;

public sealed class UpdateProductUseCase : IUseCase<UpdateProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponse> ExecuteAsync(UpdateProductCommand input, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new ProductNotFoundException(input.Id);

        product.UpdateDetails(input.Name, input.Description, input.Price, input.Stock, input.CategoryId);
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return new ProductResponse(product.Id, product.Name, product.Description, product.Price, product.Stock, product.CategoryId, product.ImageUrl, product.CreatedAtUtc);
    }
}