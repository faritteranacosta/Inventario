using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Products.Dtos;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Domain.Products.Exceptions;

namespace Unimag.Inventario.Application.Products.Queries.GetProductByBarcode;

public sealed class GetProductByBarcodeUseCase
    : IUseCase<GetProductByBarcodeQuery, ProductResponse>
{
    private readonly IProductRepository _repository;

    public GetProductByBarcodeUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductResponse> ExecuteAsync(
        GetProductByBarcodeQuery input,
        CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByBarcodeAsync(input.Barcode, cancellationToken)
            ?? throw new ProductBarcodeNotFoundException(input.Barcode);

        return new ProductResponse(
            product.Id, product.Name, product.Description, product.Price,
            product.Stock, product.CategoryId, product.ImageUrl,
            product.MinimumStock, product.Barcode, product.CreatedAtUtc);
    }
}
