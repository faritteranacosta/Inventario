using Unimag.Inventario.Application.Sales.Dtos;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Domain.Products.Exceptions;
using Unimag.Inventario.Domain.Sales;
using Unimag.Inventario.Domain.Sales.Contracts;
using Unimag.Inventario.Domain.Sales.Exceptions;
using Unimag.Inventario.Domain.Tenants.Contracts;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Application.Sales.Commands.CreateSale;

public sealed class CreateSaleUseCase : IUseCase<CreateSaleCommand, SaleResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ITransactionManager _transactionManager;

    public CreateSaleUseCase(IProductRepository productRepository, ISaleRepository saleRepository,
        ITenantRepository tenantRepository, ITransactionManager? transactionManager = null)
    {
        _productRepository = productRepository;
        _saleRepository = saleRepository;
        _tenantRepository = tenantRepository;
        _transactionManager = transactionManager ?? NoOpTransactionManager.Instance;
    }

    public Task<SaleResponse> ExecuteAsync(CreateSaleCommand input, CancellationToken cancellationToken = default)
        => _transactionManager.ExecuteAsync(ct => ExecuteCoreAsync(input, ct), cancellationToken);

    private async Task<SaleResponse> ExecuteCoreAsync(CreateSaleCommand input, CancellationToken cancellationToken)
    {
        if (input.Items is null || input.Items.Count == 0)
        {
            throw new EmptySaleException();
        }

        var idempotencyKey = string.IsNullOrWhiteSpace(input.IdempotencyKey)
            ? null
            : input.IdempotencyKey.Trim();
        var payloadHash = idempotencyKey is null ? null : ComputePayloadHash(input.Items);
        if (idempotencyKey is not null)
        {
            var existingSale = await _saleRepository.GetByIdempotencyKeyAsync(idempotencyKey, cancellationToken);
            if (existingSale is not null)
            {
                if (!string.Equals(existingSale.IdempotencyPayloadHash, payloadHash, StringComparison.Ordinal))
                {
                    throw new IdempotencyKeyPayloadMismatchException(idempotencyKey);
                }

                return ToResponse(existingSale);
            }
        }

        var saleItems = new List<SaleItem>();
        foreach (var (productId, quantity) in input.Items)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken)
                ?? throw new ProductNotFoundException(productId);

            product.DecreaseStock(quantity);
            await _productRepository.UpdateAsync(product, cancellationToken);

            saleItems.Add(new SaleItem(productId, product.Name, quantity, product.Price));
        }

        var tenant = await _tenantRepository.GetByIdAsync(input.TenantId, cancellationToken);
        var tenantName = tenant?.Name ?? string.Empty;
        var sale = Sale.Create(input.TenantId, tenantName, saleItems, idempotencyKey, payloadHash);
        await _saleRepository.AddAsync(sale, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);
        await _saleRepository.SaveChangesAsync(cancellationToken);

        return ToResponse(sale);
    }

    private static SaleResponse ToResponse(Sale sale) =>
        new(sale.Id, sale.TenantName, sale.SaleDateUtc,
            sale.Items.Select(i => new SaleItemResponse(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.Total)).ToList(),
            sale.Total);

    private static string ComputePayloadHash(IReadOnlyList<(Guid ProductId, int Quantity)> items)
    {
        var payload = string.Join(
            "|",
            items.Select(item =>
                $"{item.ProductId:D}:{item.Quantity.ToString(CultureInfo.InvariantCulture)}"));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload)));
    }

    private sealed class NoOpTransactionManager : ITransactionManager
    {
        public static NoOpTransactionManager Instance { get; } = new();

        public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default) => operation(cancellationToken);
    }
}