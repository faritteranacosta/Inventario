using Unimag.Inventario.Application.Products.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Products;
using Unimag.Inventario.Domain.Products.Contracts;

namespace Unimag.Inventario.Application.Products.Commands.CreateProduct;

public sealed class CreateProductUseCase : IUseCase<CreateProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;

    public CreateProductUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponse> ExecuteAsync(CreateProductCommand input, CancellationToken cancellationToken = default)
    {
        var product = Product.Create(input.Name, input.Description, input.Price, input.Stock, input.CategoryId, input.ImageUrl);
        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return new ProductResponse(product.Id, product.Name, product.Description, product.Price, product.Stock, product.CategoryId, product.ImageUrl, product.CreatedAtUtc);
    }
}