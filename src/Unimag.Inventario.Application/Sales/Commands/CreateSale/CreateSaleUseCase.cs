using Unimag.Inventario.Application.Sales.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Domain.Products.Exceptions;
using Unimag.Inventario.Domain.Sales;
using Unimag.Inventario.Domain.Sales.Contracts;
using Unimag.Inventario.Domain.Sales.Exceptions;

namespace Unimag.Inventario.Application.Sales.Commands.CreateSale;

public sealed class CreateSaleUseCase : IUseCase<CreateSaleCommand, SaleResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ISaleRepository _saleRepository;

    public CreateSaleUseCase(IProductRepository productRepository, ISaleRepository saleRepository)
    {
        _productRepository = productRepository;
        _saleRepository = saleRepository;
    }

    public async Task<SaleResponse> ExecuteAsync(CreateSaleCommand input, CancellationToken cancellationToken = default)
    {
        if (input.Items is null || input.Items.Count == 0)
        {
            throw new EmptySaleException();
        }

        var saleItems = new List<SaleItem>();
        foreach (var (productId, quantity) in input.Items)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken)
                ?? throw new ProductNotFoundException(productId);

            product.DecreaseStock(quantity);
            await _productRepository.UpdateAsync(product, cancellationToken);

            saleItems.Add(new SaleItem(productId, quantity, product.Price));
        }

        var sale = Sale.Create(input.TenantId, saleItems);
        await _saleRepository.AddAsync(sale, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);
        await _saleRepository.SaveChangesAsync(cancellationToken);

        return ToResponse(sale);
    }

    private static SaleResponse ToResponse(Sale sale) =>
        new(sale.Id, sale.SaleDateUtc,
            sale.Items.Select(i => new SaleItemResponse(i.ProductId, i.Quantity, i.UnitPrice, i.Total)).ToList(),
            sale.Total);
}