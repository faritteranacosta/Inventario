using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.InventoryMovements.Commands.CreateInventoryMovement;
using Unimag.Inventario.Application.Purchases.Commands.CreatePurchase;
using Unimag.Inventario.Application.Purchases.Dtos;
using Unimag.Inventario.Domain.InventoryMovements;
using Unimag.Inventario.Domain.InventoryMovements.Contracts;
using Unimag.Inventario.Domain.Purchases;
using Unimag.Inventario.Domain.Purchases.Contracts;
using Unimag.Inventario.Domain.Purchases.Exceptions;
using Unimag.Inventario.Domain.Products.Contracts;
using Unimag.Inventario.Domain.Products.Exceptions;
using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Application.Purchases.Commands.ChangePurchaseStatus;

public sealed class ChangePurchaseStatusUseCase
    : IUseCase<ChangePurchaseStatusCommand, PurchaseResponse>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly ITransactionManager _transactionManager;

    public ChangePurchaseStatusUseCase(
        IPurchaseRepository purchaseRepository,
        IProductRepository productRepository,
        IInventoryMovementRepository movementRepository,
        ITransactionManager? transactionManager = null)
    {
        _purchaseRepository = purchaseRepository;
        _productRepository = productRepository;
        _movementRepository = movementRepository;
        _transactionManager = transactionManager ?? NoOpTransactionManager.Instance;
    }

    public async Task<PurchaseResponse> ExecuteAsync(
        ChangePurchaseStatusCommand input,
        CancellationToken cancellationToken = default)
    {
        return await _transactionManager.ExecuteAsync(
            ct => ExecuteCoreAsync(input, ct), cancellationToken);
    }

    private async Task<PurchaseResponse> ExecuteCoreAsync(
        ChangePurchaseStatusCommand input, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new PurchaseNotFoundException(input.Id);

        if (input.Status == PurchaseStatus.Received)
        {
            purchase.Receive();
            foreach (var item in purchase.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
                    ?? throw new ProductNotFoundException(item.ProductId);
                var before = product.Stock;
                product.IncreaseStock(item.Quantity);
                await _productRepository.UpdateAsync(product, cancellationToken);
                var movement = InventoryMovement.Create(
                    product.TenantId, product.Id, InventoryMovementType.In,
                    item.Quantity, before, "Purchase received", "PURCHASE",
                    purchase.Id, input.UserId);
                await _movementRepository.AddAsync(movement, cancellationToken);
            }
        }
        else if (input.Status == PurchaseStatus.Cancelled)
        {
            purchase.Cancel();
        }
        else
        {
            throw new PurchaseStatusTransitionException(purchase.Status, input.Status);
        }

        await _purchaseRepository.UpdateAsync(purchase, cancellationToken);
        await _purchaseRepository.SaveChangesAsync(cancellationToken);
        return CreatePurchaseUseCase.ToResponse(purchase);
    }

    private sealed class NoOpTransactionManager : ITransactionManager
    {
        public static NoOpTransactionManager Instance { get; } = new();

        public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default) => operation(cancellationToken);
    }
}
