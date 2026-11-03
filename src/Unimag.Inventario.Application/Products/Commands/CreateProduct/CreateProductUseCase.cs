using Unimag.Inventario.Application.Products.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Products;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Domain.Products.Exceptions;

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
        if (input.Barcode is not null && await _productRepository.BarcodeExistsAsync(input.Barcode, cancellationToken: cancellationToken))
        {
            throw new BarcodeAlreadyExistsException(input.Barcode.Trim());
        }

        var product = Product.Create(input.TenantId, input.Name, input.Description, input.Price, input.Stock, input.CategoryId, input.ImageUrl, input.MinimumStock, input.Barcode);
        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return new ProductResponse(product.Id, product.Name, product.Description, product.Price, product.Stock, product.CategoryId, product.ImageUrl, product.MinimumStock, product.Barcode, product.CreatedAtUtc);
    }
}