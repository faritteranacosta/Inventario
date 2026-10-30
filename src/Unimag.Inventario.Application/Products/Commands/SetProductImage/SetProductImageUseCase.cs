using Unimag.Inventario.Application.Products.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Domain.Products.Exceptions;

namespace Unimag.Inventario.Application.Products.Commands.SetProductImage;

public sealed class SetProductImageUseCase : IUseCase<SetProductImageCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;

    public SetProductImageUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponse> ExecuteAsync(SetProductImageCommand input, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new ProductNotFoundException(input.Id);

        product.SetImage(input.ImageUrl);
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return new ProductResponse(product.Id, product.Name, product.Description, product.Price, product.Stock, product.CategoryId, product.ImageUrl, product.CreatedAtUtc);
    }
}