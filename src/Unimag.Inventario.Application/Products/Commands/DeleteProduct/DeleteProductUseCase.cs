using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Domain.Products.Exceptions;

namespace Unimag.Inventario.Application.Products.Commands.DeleteProduct;

public sealed class DeleteProductUseCase : IUseCase<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<bool> ExecuteAsync(DeleteProductCommand input, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new ProductNotFoundException(input.Id);

        await _productRepository.DeleteAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}